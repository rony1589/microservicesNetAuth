using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Middleware;

public sealed class CorrelationIdMiddleware : IMiddleware
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var fromHeader = context.Request.Headers[HeaderName].FirstOrDefault();
        var correlationId = string.IsNullOrWhiteSpace(fromHeader) ? Guid.NewGuid().ToString() : fromHeader!;
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await next(context);
    }
}
