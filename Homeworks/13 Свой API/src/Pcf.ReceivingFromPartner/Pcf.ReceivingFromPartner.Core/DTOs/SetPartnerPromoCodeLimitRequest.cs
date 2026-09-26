using System;

namespace Pcf.ReceivingFromPartner.Core.DTOs
{
    public class SetPartnerPromoCodeLimitRequest
    {
        public DateTime EndDate { get; set; }
        public int Limit { get; set; }
    }
}