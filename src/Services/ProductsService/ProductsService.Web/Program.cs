using System.Reflection;
using System.Text;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;

using BuildingBlocks.Auth;
using BuildingBlocks.Errors;
using BuildingBlocks.Middleware;

var builder = WebApplication.CreateBuilder(args);

// ------------------------- Configuración -------------------------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)  // ← toma appsettings.json
    .AddEnvironmentVariables();

// ------------------------- Serilog -------------------------------
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/products-service-.log", rollingInterval: RollingInterval.Day)
);

// Ver problemas de configuración de Serilog (sinks, permisos, etc.)
Serilog.Debugging.SelfLog.Enable(Console.Error);

// ------------------------- DB -----------------------------------
var productsConn = builder.Configuration.GetConnectionString("ProductsDb");
ArgumentException.ThrowIfNullOrWhiteSpace(productsConn, "ConnectionStrings:ProductsDb");

builder.Services.AddDbContext<ProductsService.Infrastructure.ProductsDbContext>(opt =>
    opt.UseNpgsql(productsConn));

// ------------------------- Infra --------------------------------
builder.Services.AddScoped<
    ProductsService.Application.Abstractions.IProductRepository,
    ProductsService.Infrastructure.ProductRepository>();

builder.Services.AddScoped<
    ProductsService.Application.Abstractions.IUnitOfWork,
    ProductsService.Infrastructure.UnitOfWork>();

// ------------------------- CQRS, Mapper, Validations -------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(Assembly.Load("ProductsService.Application")));

builder.Services.AddAutoMapper(Assembly.Load("ProductsService.Application"));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.Load("ProductsService.Application"));

// ------------------------- JWT (compartido) ----------------------
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey), "Jwt:SigningKey es obligatorio.")
    .ValidateOnStart();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Sección 'Jwt' no encontrada.");

var signingKeyStr = jwt.SigningKey;
ArgumentException.ThrowIfNullOrWhiteSpace(signingKeyStr, "Jwt:SigningKey");
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

        // ProblemDetails 401 / 403
        o.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                ctx.HandleResponse();
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                ctx.Response.ContentType = "application/problem+json";

                var pb = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/401",
                    Title = "No autenticado",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = "Proporcione un token Bearer válido en el encabezado Authorization.",
                    Instance = ctx.Request.Path
                };
                pb.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;

                await ctx.Response.WriteAsJsonAsync(pb);
            },
            OnForbidden = async ctx =>
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                ctx.Response.ContentType = "application/problem+json";

                var pb = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/403",
                    Title = "Prohibido",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = "El token es válido, pero su rol no permite esta operación.",
                    Instance = ctx.Request.Path
                };
                pb.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;

                await ctx.Response.WriteAsJsonAsync(pb);
            }
        };
    });

builder.Services.AddAuthorization();

// ------------------------- Controllers + ModelState→Problem -------
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

// ------------------------- Middlewares compartidos ----------------
builder.Services.AddTransient<CorrelationIdMiddleware>();
builder.Services.AddTransient<ErrorHandlingMiddleware>();

var app = builder.Build();

// ------------------------- Pipeline -------------------------------
app.UseSerilogRequestLogging(); // logs por request

app.UseSwagger();
app.UseSwaggerUI();

// Coloca Correlation + ErrorHandling al inicio para capturar TODO (incluye auth)
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() => Serilog.Debugging.SelfLog.Disable());

app.Run();