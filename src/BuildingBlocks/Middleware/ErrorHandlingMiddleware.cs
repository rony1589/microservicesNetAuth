using System.Net;
using System.Text.Json;
using BuildingBlocks.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Middleware;

public sealed class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string correlationId = context.Items.TryGetValue("CorrelationId", out var v) ? v?.ToString()! : Guid.NewGuid().ToString();
        try
        {
            await next(context);
        }
        catch (ValidationAppException vex)
        {
            await WriteProblem(context, vex.Status, vex.Message, vex, correlationId, extra: new Dictionary<string, object?>
            {
                ["errors"] = vex.Errors
            });
        }
        catch (AppException aex)
        {
            await WriteProblem(context, aex.Status, aex.Message, aex, correlationId);
        }
        catch (UnauthorizedAccessException uex)
        {
            var ex = new UnauthorizedAppException(ErrorCode.AuthUnauthorized, uex.Message);
            await WriteProblem(context, ex.Status, ex.Message, ex, correlationId);
        }
        catch (Exception ex)
        {
            await WriteProblem(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.", ex, correlationId,
                errorCode: ErrorCode.Unexpected);
        }
    }

    private async Task WriteProblem(
        HttpContext ctx,
        HttpStatusCode status,
        string message,
        Exception ex,
        string correlationId,
        string? errorCode = null,
        IDictionary<string, object?>? extra = null)
    {
        var traceId = ctx.TraceIdentifier;
        var code = errorCode ?? (ex as AppException)?.Code ?? ErrorCode.Unexpected;
        var type = code switch
        {
            ErrorCode.AuthInvalidCredentials => "https://errors.yourdomain.com/auth/invalid-credentials",
            ErrorCode.ValidationFailed => "https://errors.yourdomain.com/common/validation-failed",
            ErrorCode.NotFound => "https://errors.yourdomain.com/common/not-found",
            ErrorCode.Conflict => "https://errors.yourdomain.com/common/conflict",
            ErrorCode.DomainRuleViolated => "https://errors.yourdomain.com/common/domain-rule",
            ErrorCode.AuthUnauthorized => "https://errors.yourdomain.com/auth/unauthorized",
            ErrorCode.AuthForbidden => "https://errors.yourdomain.com/auth/forbidden",
            _ => "https://errors.yourdomain.com/common/unexpected"
        };

        var logLevel = (int)status >= 500 ? LogLevel.Error : LogLevel.Information;
        _logger.Log(logLevel, ex, "Error handled {@Status} {@ErrorCode} {@CorrelationId} {@TraceId} {@Path}",
            (int)status, code, correlationId, traceId, ctx.Request.Path);

        var problem = new
        {
            type,
            title = message,
            status = (int)status,
            detail = (int)status >= 500 ? null : message,
            instance = ctx.Request.Path,
            extensions = Merge(new Dictionary<string, object?>
            {
                ["errorCode"] = code,
                ["correlationId"] = correlationId,
                ["traceId"] = traceId
            }, extra)
        };

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = (int)status;
        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await ctx.Response.WriteAsync(json);
    }

    private static IDictionary<string, object?> Merge(IDictionary<string, object?> a, IDictionary<string, object?>? b)
    {
        if (b is null) return a;
        foreach (var kv in b) a[kv.Key] = kv.Value;
        return a;
    }
}
