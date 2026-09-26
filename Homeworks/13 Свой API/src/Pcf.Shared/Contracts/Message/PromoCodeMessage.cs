namespace Pcf.Shared.Contracts.Message;

public class PromoCodeMessage
{
    public Guid PromoCodeId { get; set; }
    public required string PromoCode { get; set; }
    public required string ServiceInfo { get; set; }
    public DateTime BeginDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid PartnerId { get; set; }
    public Guid? PartnerManagerId { get; set; }
    public Guid PreferenceId { get; set; }
}
