using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Tests;

public sealed class InMemoryDeliveries : IDeliveryRepository
{
    public Dictionary<DeliveryId, Delivery> Items { get; } = new();
    public int Saves { get; private set; }

    public Task<Delivery?> GetAsync(DeliveryId id, CancellationToken ct = default) =>
        Task.FromResult(Items.GetValueOrDefault(id));

    public Task SaveAsync(Delivery delivery, CancellationToken ct = default)
    {
        Items[delivery.Id] = delivery;
        Saves++;
        return Task.CompletedTask;
    }
}

public sealed class InMemoryRoutes : IRouteRepository
{
    public Dictionary<RouteId, Route> Items { get; } = new();

    public Task<Route?> GetAsync(RouteId id, CancellationToken ct = default) =>
        Task.FromResult(Items.GetValueOrDefault(id));

    public Task SaveAsync(Route route, CancellationToken ct = default)
    {
        Items[route.Id] = route;
        return Task.CompletedTask;
    }
}

public sealed class FakeAvailability(bool available, bool fails = false) : ICourierAvailabilityPort
{
    public Task<bool> IsAvailableAsync(CourierId courierId, CancellationToken ct = default) =>
        fails ? throw new CourierAvailabilityUnavailableException() : Task.FromResult(available);
}

public sealed class RecordingPublisher : IEventPublisher
{
    public List<object> Events { get; } = new();

    public Task PublishAsync<T>(T domainEvent, CancellationToken ct = default) where T : notnull
    {
        Events.Add(domainEvent);
        return Task.CompletedTask;
    }
}

public sealed class PassThroughUnitOfWork : IUnitOfWork
{
    public Task<T> ExecuteAsync<T>(Func<Task<T>> work, CancellationToken ct = default) => work();
}
