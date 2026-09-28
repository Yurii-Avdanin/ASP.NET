using Pcf.GivingToCustomer.Core.Domain;
using System.Collections.Generic;
using System.Linq;

namespace Pcf.GivingToCustomer.Integration.DTOs;

public static class PreferenceDtoExtensions
{
    public static Preference ToPreference(this PreferenceDto dto)
        => new Preference()    
        {
            Id = dto.Id,
            Name = dto.Name
        };    

    public static IEnumerable<Preference> ToListPreferences(this IEnumerable<PreferenceDto> listDto)
        => listDto.Select(preference => preference.ToPreference());    
}
