namespace Catalog.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        //Get All
        group.MapGet("/", async (ProductService productService) =>
        {
            var products =  await productService.GetProductsAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        //Get By Id
        group.MapGet("/{id:int}", async (int id, ProductService productService) =>
        {
            var product = await productService.GetProductByIdAsync(id);
            return product is not null ? Results.Ok(product) : Results.NotFound();
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        //Create
        group.MapPost("/", async (Product product, ProductService productService) =>
        {
            await productService.CreateProductAsync(product);
            return Results.Created($"/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        //Update
        group.MapPut("/{id:int}", async (int id, Product inputProduct, ProductService productService) =>
        {
            var existingProduct = await productService.GetProductByIdAsync(id);
            if (existingProduct is null) return Results.NotFound();

            await productService.UpdateProductAsync(existingProduct, inputProduct);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        //Delete
        group.MapDelete("/{id:int}", async (int id, ProductService productService) =>
        {
            var existingProduct = await productService.GetProductByIdAsync(id);
            if (existingProduct is null) return Results.NotFound();

            await productService.DeleteProductAsync(existingProduct);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
