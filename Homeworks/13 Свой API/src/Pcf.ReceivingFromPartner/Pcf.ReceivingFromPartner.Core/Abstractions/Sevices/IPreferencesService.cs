using Pcf.ReceivingFromPartner.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;

public interface IPreferencesService
{
    /// <summary>
    /// Получить список предпочтений
    /// </summary>
    /// <returns></returns>   
    Task<List<PreferenceResponse>> GetPreferencesAsync();
}
