using LastMileHub.Application.Dispatch;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LastMileHub.Infrastructure.Persistence;

/// <summary>Conexión y transacción compartidas por los repositorios de una misma petición (Unit of Work).</summary>
public sealed class SqliteSession : IUnitOfWork, IAsyncDisposable
{
    private readonly string _connectionString;
    private SqliteConnection? _connection;
    private SqliteTransaction? _transaction;

    public SqliteSession(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default") ?? "Data Source=lastmilehub.db";
    }

    public async Task<SqliteCommand> CreateCommandAsync(string sql, CancellationToken ct)
    {
        if (_connection is null)
        {
            _connection = new SqliteConnection(_connectionString);
            await _connection.OpenAsync(ct);
        }

        var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        cmd.Transaction = _transaction;
        return cmd;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> work, CancellationToken ct = default)
    {
        await using (await CreateCommandAsync("SELECT 1", ct)) { } // asegura la conexión abierta
        _transaction = _connection!.BeginTransaction();
        try
        {
            var result = await work();
            _transaction.Commit();
            return result;
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null) await _connection.DisposeAsync();
    }
}
