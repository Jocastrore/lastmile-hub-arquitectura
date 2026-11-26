using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Application.Dispatch;

// Contrato de aplicación del caso de uso.
public sealed record AssignCourierCommand(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId);

public sealed record AssignCourierResult(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId, DeliveryStatus Status);

// Evento que otros contextos (por ejemplo Notificaciones) consumen.
public sealed record DeliveryAssigned(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId, DateTimeOffset OccurredAt);

public sealed class AssignCourierUseCase
{
    private readonly IDeliveryRepository _deliveries;
    private readonly IRouteRepository _routes;
    private readonly ICourierAvailabilityPort _availability;
    private readonly IEventPublisher _events;
    private readonly IUnitOfWork _uow;
    private readonly TimeProvider _clock;

    // Solo recibe abstracciones: no conoce SQL, HTTP ni colas.
    public AssignCourierUseCase(
        IDeliveryRepository deliveries,
        IRouteRepository routes,
        ICourierAvailabilityPort availability,
        IEventPublisher events,
        IUnitOfWork uow,
        TimeProvider clock)
    {
        _deliveries = deliveries;
        _routes = routes;
        _availability = availability;
        _events = events;
        _uow = uow;
        _clock = clock;
    }

    public async Task<AssignCourierResult> ExecuteAsync(AssignCourierCommand cmd, CancellationToken ct = default)
    {
        var delivery = await _deliveries.GetAsync(cmd.DeliveryId, ct)
            ?? throw new DeliveryNotFoundException(cmd.DeliveryId);

        var route = await _routes.GetAsync(cmd.RouteId, ct)
            ?? throw new RouteNotFoundException(cmd.RouteId);

        if (!await _availability.IsAvailableAsync(cmd.CourierId, ct))     // INV-04
            throw new CourierNotAvailableException(cmd.CourierId);

        delivery.AssignCourier(cmd.CourierId, cmd.RouteId);               // INV-01, INV-02, INV-06
        route.AddDelivery(delivery.Id);                                   // INV-03

        await _uow.ExecuteAsync(async () =>
        {
            await _deliveries.SaveAsync(delivery, ct);
            await _routes.SaveAsync(route, ct);
            await _events.PublishAsync(
                new DeliveryAssigned(delivery.Id, cmd.CourierId, cmd.RouteId, _clock.GetUtcNow()), ct);
            return true;
        }, ct);

        return new AssignCourierResult(delivery.Id, cmd.CourierId, cmd.RouteId, delivery.Status);
    }
}
