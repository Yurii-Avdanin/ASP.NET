using Pcf.ReceivingFromPartner.Application.Mappers;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;
using Pcf.ReceivingFromPartner.Core.DTOs;

namespace Pcf.ReceivingFromPartner.Application.Sevices;

public class PreferencesService : IPreferencesService
{    
    private readonly IPreferencesGateway _preferencesGateway;

    public PreferencesService(IPreferencesGateway preferencesGateway)
    {
        _preferencesGateway = preferencesGateway;
    }

    /// <summary>
    /// Получить список предпочтений
    /// </summary>
    /// <returns></returns>   
    public async Task<List<PreferenceResponse>> GetPreferencesAsync()
    {
        var preferences = await _preferencesGateway.GetAllAsync();

        return preferences.ToPreferenceResponse().ToList();
    }
}
