using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Basket.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler(BasketService basketService) : IConsumer<ProductPriceChangedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
    {
        var integrationEvent = context.Message;
        await basketService.UpdateBasketItemProductPrices(integrationEvent.ProductId, integrationEvent.Price);
    }
}
