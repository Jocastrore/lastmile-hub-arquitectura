namespace LastMileHub.Domain.Dispatch;

public sealed class Delivery
{
    public DeliveryId Id { get; }
    public DeliveryAddress Address { get; }
    public DeliveryStatus Status { get; private set; }
    public CourierId? CourierId { get; private set; }
    public RouteId? RouteId { get; private set; }

    public Delivery(
        DeliveryId id,
        DeliveryAddress address,
        DeliveryStatus status,
        CourierId? courierId = null,
        RouteId? routeId = null)
    {
        Id = id;
        Address = address;
        Status = status;
        CourierId = courierId;
        RouteId = routeId;
    }

    public void AssignCourier(CourierId courierId, RouteId routeId)
    {
        if (CourierId is not null)
            throw new DeliveryAlreadyAssignedException(Id);               // INV-01

        if (Status != DeliveryStatus.ReadyForDispatch)
            throw new DeliveryNotAssignableException(Id, Status);         // INV-02, INV-06

        CourierId = courierId;
        RouteId = routeId;
        Status = DeliveryStatus.Assigned;
    }
}
