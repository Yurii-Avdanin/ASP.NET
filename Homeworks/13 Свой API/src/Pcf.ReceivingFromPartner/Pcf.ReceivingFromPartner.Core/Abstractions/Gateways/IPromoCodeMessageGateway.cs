using Pcf.Shared.Contracts.Message;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;

public interface IPromoCodeMessageGateway
{
    Task PublishPromoCodeAsync(PromoCodeMessage @event);
}
