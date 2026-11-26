using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Application.Dispatch;

public interface IDeliveryRepository
{
    Task<Delivery?> GetAsync(DeliveryId id, CancellationToken ct = default);
    Task SaveAsync(Delivery delivery, CancellationToken ct = default);
}

public interface IRouteRepository
{
    Task<Route?> GetAsync(RouteId id, CancellationToken ct = default);
    Task SaveAsync(Route route, CancellationToken ct = default);
}

/// <summary>Puerto hacia el sistema de turnos (INV-04).</summary>
public interface ICourierAvailabilityPort
{
    Task<bool> IsAvailableAsync(CourierId courierId, CancellationToken ct = default);
}

public interface IEventPublisher
{
    Task PublishAsync<T>(T domainEvent, CancellationToken ct = default) where T : notnull;
}

public interface IUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> work, CancellationToken ct = default);
}
