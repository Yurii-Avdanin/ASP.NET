using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;

namespace Pcf.ReceivingFromPartner.Application.Mappers;

public static class PartnerExtensions
{
    public static PartnerResponse ToPartnerResponse(this Partner partner)
        => new()
        {
            Id = partner.Id,
            Name = partner.Name,
            NumberIssuedPromoCodes = partner.NumberIssuedPromoCodes,
            IsActive = true,
            PartnerLimits = partner.PartnerLimits?
                .Select(y => y.ToPartnerPromoCodeLimitResponse())
                .ToList() ?? []
        };    

    public static IEnumerable<PartnerResponse> ToPartnerResponse(this IEnumerable<Partner> partners)
       => partners.Select(p => p.ToPartnerResponse());    
}
