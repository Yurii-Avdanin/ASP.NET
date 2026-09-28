using System;
using System.Collections.Generic;

namespace Pcf.ReceivingFromPartner.Core.DTOs;

public class PartnerResponse
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }

    public string Name { get; set; }

    public int NumberIssuedPromoCodes { get; set; }

    public List<PartnerPromoCodeLimitResponse> PartnerLimits { get; set; }
}