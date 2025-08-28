using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UsersService.Application.Abstractions;
using BuildingBlocks.Middleware;
using BuildingBlocks.Errors;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using BuildingBlocks.Auth;

var builder = WebApplication.CreateBuilder(args);

// Serilog POR CÓDIGO (sin JSON)
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/users-service-.log", rollingInterval: RollingInterval.Day)
);

// habilita ver errores de configuración de Serilog en consola (útil si algo falla)
Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Configuration.AddEnvironmentVariables();

// DB
var usersConn = builder.Configuration.GetConnectionString("UsersDb");
ArgumentException.ThrowIfNullOrWhiteSpace(usersConn, "ConnectionStrings:UsersDb");

builder.Services.AddDbContext<UsersService.Infrastructure.UsersDbContext>(opt =>
    opt.UseNpgsql(usersConn));

// Infra
builder.Services.AddScoped<IUserRepository, UsersService.Infrastructure.UserRepository>();
builder.Services.AddScoped<IUnitOfWork, UsersService.Infrastructure.UnitOfWork>();
builder.Services.AddSingleton<IPasswordHasher, UsersService.Infrastructure.BcryptPasswordHasher>();
builder.Services.AddSingleton<IJwtTokenFactory, UsersService.Infrastructure.JwtTokenFactory>();

// CQRS + Mapper + Validators
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("UsersService.Application")));
builder.Services.AddAutoMapper(Assembly.Load("UsersService.Application"));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.Load("UsersService.Application"));

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

// Controllers + ProblemDetails (400 uniforme)
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
builder.Services.AddSwaggerGen(c =>
{
    c.MapType<ProblemDetails>(() => new Microsoft.OpenApi.Models.OpenApiSchema
    {
        Type = "object",
        Properties =
        {
            ["type"]      = new() { Type = "string" },
            ["title"]     = new() { Type = "string" },
            ["status"]    = new() { Type = "integer", Format = "int32" },
            ["detail"]    = new() { Type = "string", Nullable = true },
            ["instance"]  = new() { Type = "string" },
            ["extensions"]= new() { Type = "object" }
        }
    });
});

// Middlewares compartidos
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Correlation + ErrorHandling al inicio
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();