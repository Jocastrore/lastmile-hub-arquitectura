using LastMileHub.Application.Dispatch;
using LastMileHub.Domain.Dispatch;

namespace LastMileHub.Api;

public static class DeliveryEndpoints
{
    public static void MapDeliveryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/deliveries");
        group.MapGet("/{id:guid}", GetDelivery);
        group.MapPost("/{id:guid}/assignment", AssignCourier);
    }

    private static async Task<IResult> GetDelivery(Guid id, IDeliveryRepository repository, CancellationToken ct)
    {
        var delivery = await repository.GetAsync(new DeliveryId(id), ct);
        if (delivery is null)
            return Results.Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Delivery not found",
                extensions: new Dictionary<string, object?> { ["code"] = "DELIVERY_NOT_FOUND" });

        return Results.Ok(new DeliveryResponse(
            delivery.Id.Value, delivery.Status.ToString(), delivery.CourierId?.Value, delivery.RouteId?.Value));
    }

    private static async Task<IResult> AssignCourier(
        Guid id, AssignCourierRequest request, AssignCourierUseCase useCase, CancellationToken ct)
    {
        if (request.CourierId == Guid.Empty || request.RouteId == Guid.Empty)
            return Results.Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Validation error",
                detail: "courierId and routeId are required.",
                extensions: new Dictionary<string, object?> { ["code"] = "VALIDATION_ERROR" });

        var result = await useCase.ExecuteAsync(
            new AssignCourierCommand(new DeliveryId(id), new CourierId(request.CourierId), new RouteId(request.RouteId)),
            ct);

        return Results.Ok(new AssignCourierResponse(
            result.DeliveryId.Value, result.CourierId.Value, result.RouteId.Value, result.Status.ToString()));
    }
}
