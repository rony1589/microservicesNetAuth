using System.Text;
using BuildingBlocks.Auth;
using BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ------------------------- Serilog (código) -------------------------
builder.Host.UseSerilog((ctx, lc) => lc
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/gateway-.log", rollingInterval: RollingInterval.Day)
);

// Debug de Serilog (ver problemas de configuración/sinks)
Serilog.Debugging.SelfLog.Enable(Console.Error);

// ------------------- Config (appsettings + ocelot) ------------------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// ------------------------- JWT Options ------------------------------
builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection("Jwt"))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SigningKey), "Jwt:SigningKey es obligatorio.")
    .ValidateOnStart();

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Sección 'Jwt' no encontrada.");

var signingKeyStr = jwt.SigningKey;
ArgumentException.ThrowIfNullOrWhiteSpace(signingKeyStr, "Jwt:SigningKey");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyStr));

// ---------------- Authentication + ProblemDetails 401/403 -----------
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

        o.Events = new JwtBearerEvents
        {
            OnChallenge = async ctx =>
            {
                // Evita respuesta por defecto (WWW-Authenticate)
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
                    Detail = "El token es válido, pero no tiene permisos suficientes en el Gateway.",
                    Instance = ctx.Request.Path
                };
                pb.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;

                await ctx.Response.WriteAsJsonAsync(pb);
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOcelot();

// CorrelationId compartido (para logs y trazabilidad)
builder.Services.AddTransient<CorrelationIdMiddleware>();

var app = builder.Build();

// -------------------------- Pipeline -------------------------------
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CorrelationIdMiddleware>();

// Manejo global de excepciones del gateway -> 502 ProblemDetails
app.UseExceptionHandler(handler =>
{
    handler.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;

        var problem = new ProblemDetails
        {
            Type = "https://errors.local/gateway/unexpected",
            Title = "Unexpected error in API Gateway",
            Status = StatusCodes.Status502BadGateway,
            Detail = ex is null ? "Bad gateway." : ex.Message,
            Instance = ctx.Request.Path
        };
        problem.Extensions["traceId"] = ctx.TraceIdentifier;
        problem.Extensions["correlationId"] = ctx.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString();

        ctx.Response.StatusCode = StatusCodes.Status502BadGateway;
        ctx.Response.ContentType = "application/problem+json";
        await ctx.Response.WriteAsJsonAsync(problem);
    });
});

// Fallback por si algún middleware deja 401/403 sin cuerpo
app.Use(async (ctx, next) =>
{
    await next();

    if ((ctx.Response.StatusCode == StatusCodes.Status401Unauthorized ||
         ctx.Response.StatusCode == StatusCodes.Status403Forbidden) &&
        !ctx.Response.HasStarted &&
        string.IsNullOrEmpty(ctx.Response.ContentType))
    {
        var status = ctx.Response.StatusCode;
        ctx.Response.ContentType = "application/problem+json";

        var pb = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{status}",
            Title = status == 401 ? "No autenticado" : "Prohibido",
            Status = status,
            Detail = status == 401
                ? "Proporcione un token Bearer válido en el encabezado Authorization."
                : "Su rol no tiene permiso para esta operación.",
            Instance = ctx.Request.Path
        };
        pb.Extensions["traceId"] = ctx.TraceIdentifier;

        await ctx.Response.WriteAsJsonAsync(pb);
    }
});

await app.UseOcelot();

// Deshabilitar SelfLog al finalizar (limpio)
app.Lifetime.ApplicationStopping.Register(() => Serilog.Debugging.SelfLog.Disable());

app.Run();
