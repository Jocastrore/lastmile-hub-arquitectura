namespace LastMileHub.Domain.Dispatch;

public abstract class DomainException : Exception
{
    protected DomainException(string code, string message) : base(message) => Code = code;
    public string Code { get; }
}

/// <summary>INV-01: una entrega solo tiene un repartidor activo.</summary>
public sealed class DeliveryAlreadyAssignedException(DeliveryId id)
    : DomainException("DELIVERY_ALREADY_ASSIGNED", $"Delivery {id} already has a courier.");

/// <summary>INV-02 / INV-06: solo se asigna una entrega lista para despacho.</summary>
public sealed class DeliveryNotAssignableException(DeliveryId id, DeliveryStatus status)
    : DomainException("DELIVERY_NOT_ASSIGNABLE", $"Delivery {id} is in status {status} and cannot be assigned.");

/// <summary>INV-03: una ruta no puede superar su capacidad.</summary>
public sealed class RouteSaturatedException(RouteId id)
    : DomainException("ROUTE_SATURATED", $"Route {id} has reached its maximum capacity.");
