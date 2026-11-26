namespace LastMileHub.Domain.Dispatch;

public sealed class Route
{
    private readonly List<DeliveryId> _deliveries;

    public RouteId Id { get; }
    public int Capacity { get; }
    public IReadOnlyCollection<DeliveryId> DeliveryIds => _deliveries;

    public Route(RouteId id, int capacity, IEnumerable<DeliveryId>? deliveries = null)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");
        Id = id;
        Capacity = capacity;
        _deliveries = deliveries?.ToList() ?? new List<DeliveryId>();
    }

    public void AddDelivery(DeliveryId deliveryId)
    {
        if (_deliveries.Count >= Capacity)
            throw new RouteSaturatedException(Id);                        // INV-03
        _deliveries.Add(deliveryId);
    }
}
