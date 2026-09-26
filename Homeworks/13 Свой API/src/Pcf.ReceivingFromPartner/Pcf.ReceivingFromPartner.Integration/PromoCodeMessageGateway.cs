using MassTransit;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.Shared.Contracts.Message;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

public class PromoCodeMessageGateway : IPromoCodeMessageGateway
{
    private readonly IPublishEndpoint _publishEndpoint;

    public PromoCodeMessageGateway(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public Task PublishPromoCodeAsync(PromoCodeMessage @event)
    {
        return _publishEndpoint.Publish(@event);
    }
}
