using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.Services;

public class BasketService(IDistributedCache cache)
{
    public async Task<ShoppingCart?> GetBasketAsync(string userName)
    {
        var basket = await cache.GetStringAsync(userName);
        return string.IsNullOrEmpty(basket) ? null :
            JsonSerializer.Deserialize<ShoppingCart>(basket);
    }

    public async Task UpdateBasketAsync(ShoppingCart basket)
    {
        await cache.SetStringAsync(basket.UserName, JsonSerializer.Serialize(basket));
    }

    public async Task DeleteBasketAsync(string userName)
    {
        await cache.RemoveAsync(userName);
    }
}
