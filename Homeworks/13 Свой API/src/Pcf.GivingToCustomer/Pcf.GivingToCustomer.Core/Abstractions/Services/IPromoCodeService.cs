using Pcf.GivingToCustomer.Core.DTOs;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Services;

public interface IPromoCodeService
{
    Task GivePromoCodesToCustomersWithPreferenceAsync(PromoCodeRequestDto request);
}
