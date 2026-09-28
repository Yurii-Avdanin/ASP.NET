using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Application.Mappers;

public static class PreferenceExtensions
{
    public static PreferenceResponse ToPreferenceResponse(this Preference preference)
        => new()
        {
            Id = preference.Id,
            Name = preference.Name
        };

    public static IEnumerable<PreferenceResponse> ToPreferenceResponse(this IEnumerable<Preference> preferences)
        => preferences.Select(p => p.ToPreferenceResponse());
}
