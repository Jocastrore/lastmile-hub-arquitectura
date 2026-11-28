using System.Reflection;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace LastMileHub.Infrastructure.Persistence;

/// <summary>Aplica scripts SQL versionados y registra cuáles ya se ejecutaron (migraciones reproducibles).</summary>
public sealed class MigrationRunner
{
    private readonly IConfiguration _config;

    public MigrationRunner(IConfiguration config) => _config = config;

    private string ConnectionString => _config.GetConnectionString("Default") ?? "Data Source=lastmilehub.db";

    public void Migrate()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        Exec(conn, null, "CREATE TABLE IF NOT EXISTS schema_version (script TEXT PRIMARY KEY, applied_at TEXT NOT NULL);");

        var applied = new HashSet<string>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT script FROM schema_version";
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) applied.Add(reader.GetString(0));
        }

        foreach (var (name, sql) in Scripts(".Migrations."))
        {
            if (applied.Contains(name)) continue;
            using var tx = conn.BeginTransaction();
            Exec(conn, tx, sql);
            Exec(conn, tx, "INSERT INTO schema_version(script, applied_at) VALUES ($n, $t)",
                ("$n", name), ("$t", DateTimeOffset.UtcNow.ToString("O")));
            tx.Commit();
        }
    }

    /// <summary>Datos de demostración. Nunca se ejecuta salvo que Seed:Demo sea true.</summary>
    public void SeedDemo()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        foreach (var (_, sql) in Scripts(".Seed."))
            Exec(conn, null, sql);
    }

    private static IEnumerable<(string Name, string Sql)> Scripts(string marker)
    {
        var asm = typeof(MigrationRunner).Assembly;
        foreach (var resource in asm.GetManifestResourceNames()
                     .Where(n => n.Contains(marker) && n.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(n => n, StringComparer.Ordinal))
        {
            using var stream = asm.GetManifestResourceStream(resource)!;
            using var reader = new StreamReader(stream);
            var name = resource[(resource.IndexOf(marker, StringComparison.Ordinal) + marker.Length)..];
            yield return (name, reader.ReadToEnd());
        }
    }

    private static void Exec(SqliteConnection conn, SqliteTransaction? tx, string sql,
        params (string Name, object Value)[] parameters)
    {
        using var cmd = conn.CreateCommand();
        cmd.Transaction = tx;
        cmd.CommandText = sql;
        foreach (var (name, value) in parameters) cmd.Parameters.AddWithValue(name, value);
        cmd.ExecuteNonQuery();
    }
}
