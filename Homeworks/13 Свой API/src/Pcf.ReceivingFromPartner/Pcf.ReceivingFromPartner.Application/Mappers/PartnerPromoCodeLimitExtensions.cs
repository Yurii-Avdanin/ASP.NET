using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Application.Mappers;

public static class PartnerPromoCodeLimitExtensions
{
    public static PartnerPromoCodeLimitResponse ToPartnerPromoCodeLimitResponse(this PartnerPromoCodeLimit limit)
    {
        return new PartnerPromoCodeLimitResponse
        {
            Id = limit.Id,
            PartnerId = limit.PartnerId,
            Limit = limit.Limit,
            CreateDate = limit.CreateDate.ToString("dd.MM.yyyy hh:mm:ss"),
            EndDate = limit.EndDate.ToString("dd.MM.yyyy hh:mm:ss"),
            CancelDate = limit.CancelDate?.ToString("dd.MM.yyyy hh:mm:ss"),
        };    
    }
}
