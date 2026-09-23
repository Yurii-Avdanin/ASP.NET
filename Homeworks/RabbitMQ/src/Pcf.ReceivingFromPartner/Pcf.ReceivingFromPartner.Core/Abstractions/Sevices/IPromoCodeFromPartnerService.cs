using Pcf.ReceivingFromPartner.Core.DTOs;
using System;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;

public interface IPromoCodeFromPartnerService
{
    Task<Guid> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid partnerId, ReceivingPromoCodeRequest request);
}
