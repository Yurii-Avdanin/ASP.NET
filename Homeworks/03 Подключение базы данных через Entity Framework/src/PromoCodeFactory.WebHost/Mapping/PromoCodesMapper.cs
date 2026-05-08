using Microsoft.AspNetCore.Http.HttpResults;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.PromoCodes;

namespace PromoCodeFactory.WebHost.Mapping;

public static class PromoCodesMapper
{
    public static PromoCodeShortResponse ToPromoCodeShortResponse(PromoCode promoCode)
    {
        return new PromoCodeShortResponse(
            promoCode.Id,
            promoCode.Code,
            promoCode.ServiceInfo,
            promoCode.PartnerName,
            promoCode.BeginDate,
            promoCode.EndDate,
            promoCode.PartnerManager.Id,
            promoCode.Preference.Id);
    }

    public static CustomerPromoCodeResponse ToCustomerPromoCodeResponse(PromoCode promoCode, CustomerPromoCode customerPromoCode)
    {
        return new CustomerPromoCodeResponse(
            Id: promoCode.Id,
            Code: promoCode.Code,
            ServiceInfo: promoCode.ServiceInfo,
            PartnerName: promoCode.PartnerName,
            BeginDate: promoCode.BeginDate,
            EndDate: promoCode.EndDate,
            PartnerManagerId: promoCode.PartnerManager.Id,
            PreferenceId: promoCode.Preference.Id,
            CreatedAt: customerPromoCode.CreatedAt,
            AppliedAt: customerPromoCode.AppliedAt);
    }
}
