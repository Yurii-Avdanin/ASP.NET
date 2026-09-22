using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Dto;
using Pcf.ReceivingFromPartner.Integration.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

public class PreferencesGateway : IPreferencesGateway
{
    private readonly HttpClient _httpClient;

    public PreferencesGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<Preference>> GetAllAsync()
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<PreferenceDto>>("api/v1/preferences");

        return dtos?.ToListPreferences().ToList() ?? Enumerable.Empty<Preference>();
    }
    public async Task<Preference> GetByIdAsync(Guid id)
    { 
        var dto = await _httpClient.GetFromJsonAsync<PreferenceDto>($"api/v1/preferences/{id}");
        return dto == null ? null : dto.ToPreference();
    }
}
