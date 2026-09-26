using Pcf.GivingToCustomer.Core.DTOs;
using Pcf.Shared.Contracts.Message;

namespace Pcf.GivingToCustomer.WebHost.Mappers;

public static class PromoCodeMessageExtensions
{
    public static PromoCodeRequestDto ToPromoCodeRequestDto(this PromoCodeMessage message)
    {
        return new PromoCodeRequestDto
        {
            ServiceInfo = message.ServiceInfo,
            PartnerId = message.PartnerId,
            PromoCodeId = message.PromoCodeId,
            PromoCode = message.PromoCode,
            PreferenceId = message.PreferenceId,
            BeginDate = message.BeginDate,
            EndDate = message.EndDate
        };
    }    
}