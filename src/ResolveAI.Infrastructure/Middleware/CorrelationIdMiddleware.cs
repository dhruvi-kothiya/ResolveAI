using Microsoft.AspNetCore.Http;

namespace ResolveAI.Infrastructure.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString();
        context.Response.Headers.Append("X-Correlation-ID", correlationId);
        await _next(context);
    }
}