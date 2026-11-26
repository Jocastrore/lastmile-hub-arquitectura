namespace LastMileHub.Domain.Dispatch;

public sealed record DeliveryId(Guid Value)
{
    public static DeliveryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record CourierId(Guid Value)
{
    public static CourierId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record RouteId(Guid Value)
{
    public static RouteId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public sealed record DeliveryAddress
{
    public string Value { get; }

    public DeliveryAddress(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Delivery address is required.", nameof(value));
        Value = value.Trim();
    }
}

public enum DeliveryStatus
{
    Pending,
    ReadyForDispatch,
    Assigned,
    InTransit,
    Delivered,
    Failed,
    Cancelled
}
