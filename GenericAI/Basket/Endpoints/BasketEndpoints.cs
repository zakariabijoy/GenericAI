namespace Basket.Endpoints;

public static class BasketEndpoints
{
    public static void MapBasketEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("basket");

        app.MapGet("/{userName}", async (string userName, BasketService basketService) =>
        {
            var shoppingCart = await basketService.GetBasketAsync(userName);
            return shoppingCart is not null ? Results.Ok(shoppingCart) : Results.NotFound();
        })
        .WithName("GetBasket")
        .Produces<ShoppingCart>(StatusCodes.Status200OK)
        .Produces<ShoppingCart>(StatusCodes.Status404NotFound);

        app.MapPost("/", async (ShoppingCart shoppingCart, BasketService basketService) =>
        {
            await basketService.UpdateBasketAsync(shoppingCart);
            return Results.Created("GetBasket", shoppingCart);
        })
        .WithName("UpdateBasket")
        .Produces<ShoppingCart>(StatusCodes.Status201Created);

        app.MapDelete("/{userName}", async (string userName, BasketService basketService) =>
        {
            await basketService.DeleteBasketAsync(userName);
            return Results.NoContent();
        })
        .WithName("DeleteBasket")
        .Produces<ShoppingCart>(StatusCodes.Status204NoContent);
    }
}
