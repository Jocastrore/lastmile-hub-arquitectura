using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Tests;

public class DeliveryTests
{
    private static Delivery Ready() =>
        new(DeliveryId.New(), new DeliveryAddress("Calle 10 # 20-30"), DeliveryStatus.ReadyForDispatch);

    [Fact]
    public void AssignCourier_when_ready_moves_to_assigned()
    {
        var delivery = Ready();
        var courier = CourierId.New();
        var route = RouteId.New();

        delivery.AssignCourier(courier, route);

        Assert.Equal(DeliveryStatus.Assigned, delivery.Status);
        Assert.Equal(courier, delivery.CourierId);
        Assert.Equal(route, delivery.RouteId);
    }

    [Fact] // INV-01
    public void AssignCourier_twice_throws_already_assigned()
    {
        var delivery = Ready();
        delivery.AssignCourier(CourierId.New(), RouteId.New());

        Assert.Throws<DeliveryAlreadyAssignedException>(() =>
            delivery.AssignCourier(CourierId.New(), RouteId.New()));
    }

    [Theory] // INV-02, INV-06
    [InlineData(DeliveryStatus.Pending)]
    [InlineData(DeliveryStatus.Cancelled)]
    [InlineData(DeliveryStatus.Delivered)]
    public void AssignCourier_when_not_ready_throws_not_assignable(DeliveryStatus status)
    {
        var delivery = new Delivery(DeliveryId.New(), new DeliveryAddress("Calle 10 # 20-30"), status);

        Assert.Throws<DeliveryNotAssignableException>(() =>
            delivery.AssignCourier(CourierId.New(), RouteId.New()));
    }

    [Fact]
    public void DeliveryAddress_rejects_empty_value()
    {
        Assert.Throws<ArgumentException>(() => new DeliveryAddress("  "));
    }
}

public class RouteTests
{
    [Fact] // INV-03
    public void AddDelivery_beyond_capacity_throws_saturated()
    {
        var route = new Route(RouteId.New(), capacity: 1);
        route.AddDelivery(DeliveryId.New());

        Assert.Throws<RouteSaturatedException>(() => route.AddDelivery(DeliveryId.New()));
    }

    [Fact]
    public void AddDelivery_within_capacity_succeeds()
    {
        var route = new Route(RouteId.New(), capacity: 2);

        route.AddDelivery(DeliveryId.New());
        route.AddDelivery(DeliveryId.New());

        Assert.Equal(2, route.DeliveryIds.Count);
    }
}
