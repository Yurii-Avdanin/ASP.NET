using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.Shared.Contracts.Message;

namespace Pcf.ReceivingFromPartner.Application.Mappers;

public static class PromoCodeMessageExtensions
{
    public static PromoCodeMessage ToPromoCodeMessage(this PromoCode promoCode)    
    {
        return new PromoCodeMessage
        {
            PromoCodeId = promoCode.Id,
            PromoCode = promoCode.Code,
            ServiceInfo = promoCode.ServiceInfo,
            PartnerId = promoCode.PartnerId,
            PreferenceId = promoCode.PreferenceId,
            BeginDate = promoCode.BeginDate,
            EndDate = promoCode.EndDate,
            PartnerManagerId = promoCode.PartnerManagerId
        };
    }
}
