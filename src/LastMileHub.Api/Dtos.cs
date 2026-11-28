namespace LastMileHub.Api;

public sealed record AssignCourierRequest(Guid CourierId, Guid RouteId);

public sealed record AssignCourierResponse(Guid DeliveryId, Guid CourierId, Guid RouteId, string Status);

// No incluye la dirección: es un dato personal que la API no necesita exponer en esta consulta.
public sealed record DeliveryResponse(Guid DeliveryId, string Status, Guid? CourierId, Guid? RouteId);
