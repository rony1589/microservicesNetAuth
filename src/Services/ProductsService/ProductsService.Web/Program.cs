using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BuildingBlocks.Middleware;
using BuildingBlocks.Errors;
using BuildingBlocks.Auth;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Serilog POR CÓDIGO (sin JSON)
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/products-service-.log", rollingInterval: RollingInterval.Day)
);

Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Configuration.AddEnvironmentVariables();

// DB
var productsConn = builder.Configuration.GetConnectionString("ProductsDb");
ArgumentException.ThrowIfNullOrWhiteSpace(productsConn, "ConnectionStrings:ProductsDb");

builder.Services.AddDbContext<ProductsService.Infrastructure.ProductsDbContext>(opt =>
    opt.UseNpgsql(productsConn));

// Infra
builder.Services.AddScoped<
    ProductsService.Application.Abstractions.IProductRepository,
    ProductsService.Infrastructure.ProductRepository>();
builder.Services.AddScoped<
    ProductsService.Application.Abstractions.IUnitOfWork,
    ProductsService.Infrastructure.UnitOfWork>();

// CQRS + Mapper + Validators
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("ProductsService.Application")));
builder.Services.AddAutoMapper(Assembly.Load("ProductsService.Application"));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.Load("ProductsService.Application"));

// ---------- JWT (compartido) ----------
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey), "Jwt:SigningKey es obligatorio.")
    .ValidateOnStart();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Sección 'Jwt' no encontrada.");

var signingKeyStr = jwt.SigningKey;
ArgumentException.ThrowIfNullOrWhiteSpace(signingKeyStr, "Jwt:SigningKey"); // evita CS8604 seguro
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyStr));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateIssuer = !string.IsNullOrWhiteSpace(jwt.Issuer),
            ValidIssuer = jwt.Issuer,

            ValidateAudience = !string.IsNullOrWhiteSpace(jwt.Audience),
            ValidAudience = jwt.Audience,

            ValidateLifetime = true
        };
    });


builder.Services.AddAuthorization();

// Controllers + ProblemDetails
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors
                    .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                    .ToArray()
            );
        throw new ValidationAppException(errors);
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Middlewares
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
