using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;
using Microsoft.Extensions.Logging.Abstractions;

namespace LastMileHub.Tests;

public class AssignCourierUseCaseTests
{
    private readonly InMemoryDeliveries _deliveries = new();
    private readonly InMemoryRoutes _routes = new();
    private readonly RecordingPublisher _events = new();

    private AssignCourierUseCase Sut(bool available = true, bool availabilityFails = false) => new(
        _deliveries, _routes, new FakeAvailability(available, availabilityFails), _events,
        new PassThroughUnitOfWork(), TimeProvider.System, NullLogger<AssignCourierUseCase>.Instance);

    private (Delivery Delivery, Route Route) Seed(int capacity = 2)
    {
        var delivery = new Delivery(DeliveryId.New(), new DeliveryAddress("Calle 10 # 20-30"), DeliveryStatus.ReadyForDispatch);
        var route = new Route(RouteId.New(), capacity);
        _deliveries.Items[delivery.Id] = delivery;
        _routes.Items[route.Id] = route;
        return (delivery, route);
    }

    [Fact]
    public async Task Success_saves_both_aggregates_and_publishes_event()
    {
        var (delivery, route) = Seed();
        var courier = CourierId.New();

        var result = await Sut().ExecuteAsync(new AssignCourierCommand(delivery.Id, courier, route.Id));

        Assert.Equal(DeliveryStatus.Assigned, result.Status);
        Assert.Equal(DeliveryStatus.Assigned, _deliveries.Items[delivery.Id].Status);
        Assert.Contains(delivery.Id, _routes.Items[route.Id].DeliveryIds);
        var published = Assert.IsType<DeliveryAssigned>(Assert.Single(_events.Events));
        Assert.Equal(courier, published.CourierId);
    }

    [Fact]
    public async Task Unknown_delivery_throws_not_found()
    {
        var (_, route) = Seed();

        await Assert.ThrowsAsync<DeliveryNotFoundException>(() =>
            Sut().ExecuteAsync(new AssignCourierCommand(DeliveryId.New(), CourierId.New(), route.Id)));
    }

    [Fact]
    public async Task Unknown_route_throws_not_found()
    {
        var (delivery, _) = Seed();

        await Assert.ThrowsAsync<RouteNotFoundException>(() =>
            Sut().ExecuteAsync(new AssignCourierCommand(delivery.Id, CourierId.New(), RouteId.New())));
    }

    [Fact] // INV-04
    public async Task Unavailable_courier_is_rejected_without_side_effects()
    {
        var (delivery, route) = Seed();

        await Assert.ThrowsAsync<CourierNotAvailableException>(() =>
            Sut(available: false).ExecuteAsync(new AssignCourierCommand(delivery.Id, CourierId.New(), route.Id)));

        Assert.Equal(0, _deliveries.Saves);
        Assert.Empty(_events.Events);
    }

    [Fact] // integración no disponible
    public async Task Availability_service_down_fails_in_a_controlled_way_without_side_effects()
    {
        var (delivery, route) = Seed();

        await Assert.ThrowsAsync<CourierAvailabilityUnavailableException>(() =>
            Sut(availabilityFails: true).ExecuteAsync(new AssignCourierCommand(delivery.Id, CourierId.New(), route.Id)));

        Assert.Equal(DeliveryStatus.ReadyForDispatch, _deliveries.Items[delivery.Id].Status);
        Assert.Empty(_events.Events);
    }

    [Fact] // INV-03
    public async Task Saturated_route_is_rejected_and_nothing_is_saved()
    {
        var (delivery, route) = Seed(capacity: 1);
        route.AddDelivery(DeliveryId.New());

        await Assert.ThrowsAsync<RouteSaturatedException>(() =>
            Sut().ExecuteAsync(new AssignCourierCommand(delivery.Id, CourierId.New(), route.Id)));

        Assert.Equal(0, _deliveries.Saves);
        Assert.Empty(_events.Events);
    }
}
