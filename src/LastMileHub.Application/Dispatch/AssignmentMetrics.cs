using System.Diagnostics.Metrics;

namespace LastMileHub.Application.Dispatch;

/// <summary>Métricas del flujo. Se pueden exportar con OpenTelemetry sin cambiar el caso de uso.</summary>
public static class AssignmentMetrics
{
    public const string MeterName = "LastMileHub.Dispatch";

    private static readonly Meter Meter = new(MeterName);
    private static readonly Counter<long> Attempts =
        Meter.CreateCounter<long>("assignments_total", description: "Intentos de asignación por resultado");
    private static readonly Histogram<double> Duration =
        Meter.CreateHistogram<double>("assignment_duration_ms", "ms", "Duración de la asignación");

    public static void Record(string result, double elapsedMs)
    {
        Attempts.Add(1, new KeyValuePair<string, object?>("result", result));
        Duration.Record(elapsedMs, new KeyValuePair<string, object?>("result", result));
    }
}
