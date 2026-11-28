using System.Text.Json;
using LastMileHub.Application.Dispatch;

namespace LastMileHub.Infrastructure.Persistence;

/// <summary>
/// Guarda el evento en la tabla outbox dentro de la misma transacción que el cambio de estado.
/// Un proceso aparte (fuera de esta actividad) publicaría los eventos a una cola real.
/// </summary>
public sealed class OutboxEventPublisher : IEventPublisher
{
    private readonly SqliteSession _session;

    public OutboxEventPublisher(SqliteSession session) => _session = session;

    public async Task PublishAsync<T>(T domainEvent, CancellationToken ct = default) where T : notnull
    {
        await using var cmd = await _session.CreateCommandAsync(
            "INSERT INTO outbox_events (id, type, payload, occurred_at) VALUES ($id, $type, $payload, $at)", ct);
        cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("$type", typeof(T).Name);
        cmd.Parameters.AddWithValue("$payload", JsonSerializer.Serialize(domainEvent));
        cmd.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O"));
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
