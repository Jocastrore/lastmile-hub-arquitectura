using LastMileHub.Api;
using LastMileHub.Application.Dispatch;
using LastMileHub.Infrastructure;
using LastMileHub.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure();
builder.Services.AddScoped<AssignCourierUseCase>();

var app = builder.Build();

// Migraciones reproducibles al arrancar; los datos de demo solo si Seed:Demo=true.
var migrations = app.Services.GetRequiredService<MigrationRunner>();
migrations.Migrate();
if (app.Configuration.GetValue<bool>("Seed:Demo"))
    migrations.SeedDemo();

app.UseCorrelationId();
app.UseErrorMapping();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.MapDeliveryEndpoints();

app.Run();

public partial class Program { }
