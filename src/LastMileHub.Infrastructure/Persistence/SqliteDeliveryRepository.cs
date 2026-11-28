using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Infrastructure.Persistence;

public sealed class SqliteDeliveryRepository : IDeliveryRepository
{
    private readonly SqliteSession _session;

    public SqliteDeliveryRepository(SqliteSession session) => _session = session;

    public async Task<Delivery?> GetAsync(DeliveryId id, CancellationToken ct = default)
    {
        await using var cmd = await _session.CreateCommandAsync(
            "SELECT status, courier_id, route_id, address FROM deliveries WHERE id = $id", ct);
        cmd.Parameters.AddWithValue("$id", id.Value.ToString());

        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;

        return new Delivery(
            id,
            new DeliveryAddress(reader.GetString(3)),
            Enum.Parse<DeliveryStatus>(reader.GetString(0)),
            reader.IsDBNull(1) ? null : new CourierId(Guid.Parse(reader.GetString(1))),
            reader.IsDBNull(2) ? null : new RouteId(Guid.Parse(reader.GetString(2))));
    }

    public async Task SaveAsync(Delivery delivery, CancellationToken ct = default)
    {
        // El WHERE protege contra dos asignaciones simultáneas de la misma entrega.
        await using var cmd = await _session.CreateCommandAsync(
            """
            INSERT INTO deliveries (id, status, courier_id, route_id, address)
            VALUES ($id, $status, $courier, $route, $address)
            ON CONFLICT(id) DO UPDATE SET
                status = excluded.status,
                courier_id = excluded.courier_id,
                route_id = excluded.route_id
            WHERE deliveries.courier_id IS NULL OR deliveries.courier_id = excluded.courier_id
            """, ct);
        cmd.Parameters.AddWithValue("$id", delivery.Id.Value.ToString());
        cmd.Parameters.AddWithValue("$status", delivery.Status.ToString());
        cmd.Parameters.AddWithValue("$courier", (object?)delivery.CourierId?.Value.ToString() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$route", (object?)delivery.RouteId?.Value.ToString() ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$address", delivery.Address.Value);

        var rows = await cmd.ExecuteNonQueryAsync(ct);
        if (rows == 0)
            throw new ConcurrencyConflictException($"Delivery {delivery.Id} was modified by another operation.");
    }
}
