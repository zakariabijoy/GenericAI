using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services;

public class ProductService(ProductDbContext dbContext, IBus bus)
{
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await dbContext.Products.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }

    public async Task CreateProductAsync(Product product)
    {
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
    {
        //if price has changed, raise ProductPriceChanged integration event
        if (updatedProduct.Price != inputProduct.Price)
        {
           // publish event
            var integrationEvent = new ProductPriceChangedIntegrationEvent
            {
                ProductId = updatedProduct.Id,
                Name = inputProduct.Name,
                Description = inputProduct.Description,
                Price  = inputProduct.Price, //set updated product price    
                ImageUrl = inputProduct.ImageUrl
            };
            await bus.Publish(integrationEvent);
        }

        // uodate product with new values
        updatedProduct.Name = inputProduct.Name;
        updatedProduct.Description = inputProduct.Description;
        updatedProduct.ImageUrl = inputProduct.ImageUrl;
        updatedProduct.Price = inputProduct.Price;

        dbContext.Products.Update(updatedProduct);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product product)
    {
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync();
    }
}
