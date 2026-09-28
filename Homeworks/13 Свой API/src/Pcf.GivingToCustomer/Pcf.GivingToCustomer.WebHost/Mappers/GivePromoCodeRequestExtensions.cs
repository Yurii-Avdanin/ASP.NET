using Pcf.GivingToCustomer.Core.DTOs;
using Pcf.GivingToCustomer.WebHost.Models;
using System;

namespace Pcf.GivingToCustomer.WebHost.Mappers;

public static class GivePromoCodeRequestExtensions
{
    public static PromoCodeRequestDto ToPromoCodeRequestDto(this GivePromoCodeRequest request)
    {
        return new PromoCodeRequestDto
        {
            ServiceInfo = request.ServiceInfo,
            PartnerId = request.PartnerId,
            PromoCodeId = request.PromoCodeId,
            PromoCode = request.PromoCode,
            PreferenceId = request.PreferenceId,
            BeginDate = DateTime.Parse(request.BeginDate),
            EndDate = DateTime.Parse(request.EndDate)
        };
    }
}
