using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Application.Dispatch;

public abstract class ApplicationErrorException : Exception
{
    protected ApplicationErrorException(string code, string message, Exception? inner = null)
        : base(message, inner) => Code = code;
    public string Code { get; }
}

public sealed class DeliveryNotFoundException(DeliveryId id)
    : ApplicationErrorException("DELIVERY_NOT_FOUND", $"Delivery {id} was not found.");

public sealed class RouteNotFoundException(RouteId id)
    : ApplicationErrorException("ROUTE_NOT_FOUND", $"Route {id} was not found.");

public sealed class CourierNotAvailableException(CourierId id)
    : ApplicationErrorException("COURIER_NOT_AVAILABLE", $"Courier {id} is not available.");

/// <summary>El sistema de turnos no respondió o falló (integración no disponible).</summary>
public sealed class CourierAvailabilityUnavailableException(Exception? inner = null)
    : ApplicationErrorException("COURIER_AVAILABILITY_UNAVAILABLE",
        "The courier availability service is not reachable. Try again later.", inner);

/// <summary>Otra operación modificó los mismos datos al mismo tiempo.</summary>
public sealed class ConcurrencyConflictException(string detail)
    : ApplicationErrorException("CONCURRENCY_CONFLICT", detail);
