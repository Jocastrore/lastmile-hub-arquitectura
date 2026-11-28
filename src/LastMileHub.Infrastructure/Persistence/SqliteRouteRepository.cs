using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Infrastructure.Persistence;

public sealed class SqliteRouteRepository : IRouteRepository
{
    private readonly SqliteSession _session;

    public SqliteRouteRepository(SqliteSession session) => _session = session;

    public async Task<Route?> GetAsync(RouteId id, CancellationToken ct = default)
    {
        int capacity;
        await using (var cmd = await _session.CreateCommandAsync("SELECT capacity FROM routes WHERE id = $id", ct))
        {
            cmd.Parameters.AddWithValue("$id", id.Value.ToString());
            var value = await cmd.ExecuteScalarAsync(ct);
            if (value is null) return null;
            capacity = Convert.ToInt32(value);
        }

        var deliveries = new List<DeliveryId>();
        await using (var cmd = await _session.CreateCommandAsync(
                         "SELECT delivery_id FROM route_deliveries WHERE route_id = $id", ct))
        {
            cmd.Parameters.AddWithValue("$id", id.Value.ToString());
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                deliveries.Add(new DeliveryId(Guid.Parse(reader.GetString(0))));
        }

        return new Route(id, capacity, deliveries);
    }

    public async Task SaveAsync(Route route, CancellationToken ct = default)
    {
        foreach (var deliveryId in route.DeliveryIds)
        {
            await using var insert = await _session.CreateCommandAsync(
                "INSERT INTO route_deliveries (route_id, delivery_id) VALUES ($r, $d) ON CONFLICT(delivery_id) DO NOTHING", ct);
            insert.Parameters.AddWithValue("$r", route.Id.Value.ToString());
            insert.Parameters.AddWithValue("$d", deliveryId.Value.ToString());
            await insert.ExecuteNonQueryAsync(ct);
        }

        // Verificación final dentro de la transacción: evita superar la capacidad por concurrencia.
        await using var count = await _session.CreateCommandAsync(
            "SELECT COUNT(*) FROM route_deliveries WHERE route_id = $r", ct);
        count.Parameters.AddWithValue("$r", route.Id.Value.ToString());
        var total = Convert.ToInt32(await count.ExecuteScalarAsync(ct));
        if (total > route.Capacity)
            throw new ConcurrencyConflictException($"Route {route.Id} exceeded its capacity concurrently.");
    }
}
