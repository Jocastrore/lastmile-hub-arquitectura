using LastMileHub.Application.Dispatch;
using LastMileHub.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace LastMileHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<MigrationRunner>();
        services.AddScoped<SqliteSession>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SqliteSession>());
        services.AddScoped<IDeliveryRepository, SqliteDeliveryRepository>();
        services.AddScoped<IRouteRepository, SqliteRouteRepository>();
        services.AddScoped<ICourierAvailabilityPort, CourierShiftAdapter>();
        services.AddScoped<IEventPublisher, OutboxEventPublisher>();
        return services;
    }
}
