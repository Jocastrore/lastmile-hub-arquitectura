using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;
using Microsoft.Extensions.DependencyInjection;

namespace LastMileHub.Tests;

/// <summary>Flujo completo: endpoint -> caso de uso -> dominio -> SQLite real (archivo temporal).</summary>
public class AssignCourierApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _client;

    public AssignCourierApiTests(ApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(); // arranca el host y aplica las migraciones
    }

    private Guid SeedDelivery(string status = "ReadyForDispatch")
    {
        var id = Guid.NewGuid();
        _factory.Exec($"INSERT INTO deliveries VALUES ('{id}', '{status}', NULL, NULL, 'Calle 10 # 20-30');");
        return id;
    }

    private Guid SeedRoute(int capacity)
    {
        var id = Guid.NewGuid();
        _factory.Exec($"INSERT INTO routes VALUES ('{id}', {capacity});");
        return id;
    }

    private Guid SeedCourier(bool onShift)
    {
        var id = Guid.NewGuid();
        _factory.Exec($"INSERT INTO courier_shifts VALUES ('{id}', {(onShift ? 1 : 0)});");
        return id;
    }

    private Task<HttpResponseMessage> Assign(Guid delivery, Guid courier, Guid route) =>
        _client.PostAsJsonAsync($"/api/deliveries/{delivery}/assignment", new { courierId = courier, routeId = route });

    private static async Task<string?> CodeOf(HttpResponseMessage response)
    {
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("code").GetString();
    }

    [Fact]
    public async Task Happy_path_assigns_persists_and_writes_outbox_event()
    {
        var delivery = SeedDelivery();
        var courier = SeedCourier(onShift: true);
        var route = SeedRoute(2);

        var response = await Assign(delivery, courier, route);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.Contains("X-Correlation-Id"));

        var get = await _client.GetFromJsonAsync<JsonElement>($"/api/deliveries/{delivery}");
        Assert.Equal("Assigned", get.GetProperty("status").GetString());
        Assert.Equal(1, _factory.Scalar($"SELECT COUNT(*) FROM route_deliveries WHERE delivery_id = '{delivery}'"));
        Assert.True(_factory.Scalar("SELECT COUNT(*) FROM outbox_events WHERE type = 'DeliveryAssigned'") >= 1);
    }

    [Fact]
    public async Task Unknown_delivery_returns_404()
    {
        var response = await Assign(Guid.NewGuid(), SeedCourier(true), SeedRoute(1));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("DELIVERY_NOT_FOUND", await CodeOf(response));
    }

    [Fact]
    public async Task Duplicate_assignment_returns_409()
    {
        var delivery = SeedDelivery();
        var courier = SeedCourier(true);
        var route = SeedRoute(2);

        Assert.Equal(HttpStatusCode.OK, (await Assign(delivery, courier, route)).StatusCode);
        var second = await Assign(delivery, SeedCourier(true), route);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        Assert.Equal("DELIVERY_ALREADY_ASSIGNED", await CodeOf(second));
    }

    [Fact]
    public async Task Saturated_route_returns_409_and_leaves_delivery_untouched()
    {
        var route = SeedRoute(1);
        var courier = SeedCourier(true);
        Assert.Equal(HttpStatusCode.OK, (await Assign(SeedDelivery(), courier, route)).StatusCode);

        var second = SeedDelivery();
        var response = await Assign(second, courier, route);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("ROUTE_SATURATED", await CodeOf(response));
        var get = await _client.GetFromJsonAsync<JsonElement>($"/api/deliveries/{second}");
        Assert.Equal("ReadyForDispatch", get.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Courier_off_shift_returns_422()
    {
        var response = await Assign(SeedDelivery(), SeedCourier(onShift: false), SeedRoute(1));

        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal("COURIER_NOT_AVAILABLE", await CodeOf(response));
    }

    [Fact]
    public async Task Cancelled_delivery_returns_409_not_assignable()
    {
        var response = await Assign(SeedDelivery("Cancelled"), SeedCourier(true), SeedRoute(1));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("DELIVERY_NOT_ASSIGNABLE", await CodeOf(response));
    }

    [Fact]
    public async Task Empty_ids_return_400()
    {
        var response = await Assign(SeedDelivery(), Guid.Empty, Guid.Empty);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("VALIDATION_ERROR", await CodeOf(response));
    }

    [Fact]
    public async Task Availability_integration_down_returns_503_without_changing_state()
    {
        var delivery = SeedDelivery();
        var route = SeedRoute(1);
        using var failing = _factory.WithWebHostBuilder(b => b.ConfigureServices(services =>
            services.AddScoped<ICourierAvailabilityPort, ThrowingAvailability>()));
        var client = failing.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/deliveries/{delivery}/assignment",
            new { courierId = Guid.NewGuid(), routeId = route });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Equal("COURIER_AVAILABILITY_UNAVAILABLE", await CodeOf(response));
        Assert.Equal(0, _factory.Scalar($"SELECT COUNT(*) FROM deliveries WHERE id = '{delivery}' AND courier_id IS NOT NULL"));
    }

    public sealed class ThrowingAvailability : ICourierAvailabilityPort
    {
        public Task<bool> IsAvailableAsync(CourierId courierId, CancellationToken ct = default) =>
            throw new CourierAvailabilityUnavailableException();
    }
}
