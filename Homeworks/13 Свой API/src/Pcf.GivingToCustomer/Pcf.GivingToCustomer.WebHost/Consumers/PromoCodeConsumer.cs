using MassTransit;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.WebHost.Mappers;
using Pcf.Shared.Contracts.Message;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Consumers;

public class PromoCodeConsumer : IConsumer<PromoCodeMessage>
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoCodeConsumer(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    public async Task Consume(ConsumeContext<PromoCodeMessage> context)
    {
        var message = context.Message;
        await _promoCodeService.GivePromoCodesToCustomersWithPreferenceAsync(message.ToPromoCodeRequestDto());
    }
}