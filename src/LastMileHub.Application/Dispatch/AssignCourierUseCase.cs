using System.Diagnostics;
using LastMileHub.Domain.Dispatch;
using Microsoft.Extensions.Logging;

namespace LastMileHub.Application.Dispatch;

public sealed record AssignCourierCommand(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId);

public sealed record AssignCourierResult(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId, DeliveryStatus Status);

public sealed record DeliveryAssigned(DeliveryId DeliveryId, CourierId CourierId, RouteId RouteId, DateTimeOffset OccurredAt);

public sealed class AssignCourierUseCase
{
    private readonly IDeliveryRepository _deliveries;
    private readonly IRouteRepository _routes;
    private readonly ICourierAvailabilityPort _availability;
    private readonly IEventPublisher _events;
    private readonly IUnitOfWork _uow;
    private readonly TimeProvider _clock;
    private readonly ILogger<AssignCourierUseCase> _logger;

    public AssignCourierUseCase(
        IDeliveryRepository deliveries,
        IRouteRepository routes,
        ICourierAvailabilityPort availability,
        IEventPublisher events,
        IUnitOfWork uow,
        TimeProvider clock,
        ILogger<AssignCourierUseCase> logger)
    {
        _deliveries = deliveries;
        _routes = routes;
        _availability = availability;
        _events = events;
        _uow = uow;
        _clock = clock;
        _logger = logger;
    }

    public async Task<AssignCourierResult> ExecuteAsync(AssignCourierCommand cmd, CancellationToken ct = default)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            var delivery = await _deliveries.GetAsync(cmd.DeliveryId, ct)
                ?? throw new DeliveryNotFoundException(cmd.DeliveryId);

            var route = await _routes.GetAsync(cmd.RouteId, ct)
                ?? throw new RouteNotFoundException(cmd.RouteId);

            // Llamada externa fuera de la transacción.
            if (!await _availability.IsAvailableAsync(cmd.CourierId, ct))
                throw new CourierNotAvailableException(cmd.CourierId);

            // Las reglas críticas viven en el dominio.
            delivery.AssignCourier(cmd.CourierId, cmd.RouteId);
            route.AddDelivery(delivery.Id);

            await _uow.ExecuteAsync(async () =>
            {
                await _deliveries.SaveAsync(delivery, ct);
                await _routes.SaveAsync(route, ct);
                await _events.PublishAsync(
                    new DeliveryAssigned(delivery.Id, cmd.CourierId, cmd.RouteId, _clock.GetUtcNow()), ct);
                return true;
            }, ct);

            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;
            AssignmentMetrics.Record("success", elapsed);
            _logger.LogInformation(
                "Courier assigned. DeliveryId={DeliveryId} CourierId={CourierId} RouteId={RouteId} ElapsedMs={ElapsedMs}",
                cmd.DeliveryId.Value, cmd.CourierId.Value, cmd.RouteId.Value, elapsed);

            return new AssignCourierResult(delivery.Id, cmd.CourierId, cmd.RouteId, delivery.Status);
        }
        catch (Exception ex) when (ex is DomainException or ApplicationErrorException)
        {
            var code = ex is DomainException d ? d.Code : ((ApplicationErrorException)ex).Code;
            AssignmentMetrics.Record(code, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
            _logger.LogWarning(
                "Assignment rejected. Code={ErrorCode} DeliveryId={DeliveryId} CourierId={CourierId} RouteId={RouteId}",
                code, cmd.DeliveryId.Value, cmd.CourierId.Value, cmd.RouteId.Value);
            throw;
        }
    }
}
