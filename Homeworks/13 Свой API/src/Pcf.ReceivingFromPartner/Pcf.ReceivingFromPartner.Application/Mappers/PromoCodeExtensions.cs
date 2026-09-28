using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;

namespace Pcf.ReceivingFromPartner.Application.Mappers;

public static class PromoCodeExtensions
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(this PromoCode promoCode)
        => new()
        {
            Id = promoCode.Id,
            Code = promoCode.Code,
            BeginDate = promoCode.BeginDate.ToString("yyyy-MM-dd"),
            EndDate = promoCode.EndDate.ToString("yyyy-MM-dd"),
            PartnerName = promoCode.Partner.Name,
            PartnerId = promoCode.PartnerId,
            ServiceInfo = promoCode.ServiceInfo
        };

    public static IEnumerable<PromoCodeShortResponse> ToPromoCodeShortResponse(this IEnumerable<PromoCode> promoCodes)
       => promoCodes.Select(p => p.ToPromoCodeShortResponse());
}