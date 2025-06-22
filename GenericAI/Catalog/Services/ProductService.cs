namespace Catalog.Services;

public class ProductService(ProductDbContext dbContext)
{
    public async Task CreateProductAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
    }
}
