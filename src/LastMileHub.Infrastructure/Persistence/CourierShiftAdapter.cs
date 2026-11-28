using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LastMileHub.Infrastructure.Persistence;

/// <summary>
/// Adaptador del sistema de turnos. En esta versión lo simula con una tabla local;
/// cualquier fallo se traduce a un error de aplicación controlado.
/// </summary>
public sealed class CourierShiftAdapter : ICourierAvailabilityPort
{
    private readonly string _connectionString;

    public CourierShiftAdapter(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default") ?? "Data Source=lastmilehub.db";
    }

    public async Task<bool> IsAvailableAsync(CourierId courierId, CancellationToken ct = default)
    {
        try
        {
            await using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync(ct);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT on_shift FROM courier_shifts WHERE courier_id = $id";
            cmd.Parameters.AddWithValue("$id", courierId.Value.ToString());
            var value = await cmd.ExecuteScalarAsync(ct);
            return value is not null && Convert.ToInt32(value) == 1;
        }
        catch (SqliteException ex)
        {
            throw new CourierAvailabilityUnavailableException(ex);
        }
    }
}
