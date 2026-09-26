using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Integration.Dto;
using System.Collections.Generic;
using System.Linq;

namespace Pcf.ReceivingFromPartner.Integration.Mappings;

public static class PreferenceDtoExtensions
{
    public static Preference ToPreference(this PreferenceDto dto)
    {
        return new Preference
        {
            Id = dto.Id,
            Name = dto.Name
        };
    }

    public static IEnumerable<Preference> ToListPreferences(this IEnumerable<PreferenceDto> listDto)
    {
        return listDto.Select(dto => dto.ToPreference());
    }
}
