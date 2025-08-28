using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using BuildingBlocks.Middleware;
using Microsoft.AspNetCore.Diagnostics;
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
    .WriteTo.File("logs/gateway-.log", rollingInterval: RollingInterval.Day)
);

Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

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
builder.Services.AddOcelot();
builder.Services.AddTransient<CorrelationIdMiddleware>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CorrelationIdMiddleware>();

// Manejo global de errores del gateway  ProblemDetails 502
app.UseExceptionHandler(handler =>
{
    handler.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
        var problem = new
        {
            type = "https://errors.local/gateway/unexpected",
            title = "Unexpected error in API Gateway",
            status = 502,
            detail = ex is null ? null : "Bad gateway.",
            instance = ctx.Request.Path,
            extensions = new
            {
                traceId = ctx.TraceIdentifier,
                correlationId = ctx.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString()
            }
        };
        ctx.Response.StatusCode = 502;
        ctx.Response.ContentType = "application/problem+json";
        await ctx.Response.WriteAsJsonAsync(problem);
    });
});

await app.UseOcelot();
app.Run();
