using System;

namespace Pcf.GivingToCustomer.Core.DTOs;

public class PromoCodeRequestDto
{
    public string ServiceInfo { get; set; }

    public Guid PartnerId { get; set; }

    public Guid PromoCodeId { get; set; }

    public string PromoCode { get; set; }

    public Guid PreferenceId { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }
}
