using System.Text.RegularExpressions;
using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Api;

public static partial class Middleware
{
    public const string CorrelationHeader = "X-Correlation-Id";

    [GeneratedRegex("^[A-Za-z0-9-]{1,64}$")]
    private static partial Regex SafeId();

    /// <summary>Asigna un id de correlación (validado para evitar inyección en logs) y lo agrega al ámbito de logging.</summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            var incoming = ctx.Request.Headers[CorrelationHeader].FirstOrDefault();
            var id = incoming is not null && SafeId().IsMatch(incoming) ? incoming : Guid.NewGuid().ToString("N");
            ctx.Items[CorrelationHeader] = id;
            ctx.Response.Headers[CorrelationHeader] = id;

            var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Request");
            using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = id }))
                await next();
        });

    /// <summary>Traduce errores de negocio e integración a respuestas controladas, sin exponer detalles internos.</summary>
    public static IApplicationBuilder UseErrorMapping(this IApplicationBuilder app) =>
        app.Use(async (ctx, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                var correlationId = ctx.Items[CorrelationHeader]?.ToString();
                var (status, code, detail) = Map(ex);

                if (status >= 500 && ex is not ApplicationErrorException)
                {
                    var logger = ctx.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Errors");
                    logger.LogError(ex, "Unhandled error. CorrelationId={CorrelationId}", correlationId);
                }

                await Results.Problem(
                    statusCode: status,
                    title: code,
                    detail: detail,
                    extensions: new Dictionary<string, object?> { ["code"] = code, ["correlationId"] = correlationId }
                ).ExecuteAsync(ctx);
            }
        });

    private static (int Status, string Code, string Detail) Map(Exception ex) => ex switch
    {
        DeliveryNotFoundException e => (StatusCodes.Status404NotFound, e.Code, e.Message),
        RouteNotFoundException e => (StatusCodes.Status404NotFound, e.Code, e.Message),
        CourierNotAvailableException e => (StatusCodes.Status422UnprocessableEntity, e.Code, e.Message),
        CourierAvailabilityUnavailableException e => (StatusCodes.Status503ServiceUnavailable, e.Code, e.Message),
        ConcurrencyConflictException e => (StatusCodes.Status409Conflict, e.Code, e.Message),
        DomainException e => (StatusCodes.Status409Conflict, e.Code, e.Message),
        _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
    };
}
