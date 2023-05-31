namespace Application.Insight.Middleware;

using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Primitives;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate next;

    public CorrelationIdMiddleware(RequestDelegate next) => this.next = next;

    public async Task Invoke(HttpContext context)
    {
        var requestTelemetry = context.Features.Get<RequestTelemetry>();

        AddCorrelationIdHeaderToResponse(context, requestTelemetry?.Context.Operation.Id);

        await next(context);
    }

    private static void AddCorrelationIdHeaderToResponse(HttpContext context, StringValues correlationId)
        => context.Response.OnStarting(() =>
        {
            context.Response.Headers.Add("X-Correlation-Id", new[] { correlationId.ToString() });
            return Task.CompletedTask;
        });
}