using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Integration.Dto;
using Pcf.GivingToCustomer.Integration.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration;

public class PreferencesGateway : IPreferencesGateway
{
    private readonly HttpClient _httpClient;

    public PreferencesGateway(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<IEnumerable<Preference>> GetAllPreferencesAsync()
    {
        var dtos = await _httpClient.GetFromJsonAsync<List<PreferenceDto>>("api/v1/preferences");

        return dtos?.ToListPreferences().ToList() ?? Enumerable.Empty<Preference>();
    }

    public async Task<Preference> GetPreferenceByIdAsync(Guid id)
    {
        var dto = await _httpClient.GetFromJsonAsync<PreferenceDto>($"api/v1/preferences/{id}");
        return dto == null ? null : dto.ToPreference();
    }

    public async Task<IEnumerable<Preference>> GetPreferencesByIdsAsync(List<Guid> ids)
    {
        var result = new List<Preference>();
        foreach (var id in ids)
        {
            var p = await GetPreferenceByIdAsync(id);

            if (p != null) 
                result.Add(p);
        }

        return result;
    }
}
