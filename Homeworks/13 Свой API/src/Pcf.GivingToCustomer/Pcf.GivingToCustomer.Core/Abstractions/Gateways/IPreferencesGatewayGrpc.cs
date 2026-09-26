using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Abstractions.Gateways;

public interface IPreferencesGatewayGrpc
{
    Task<IEnumerable<Preference>> GetAllPreferencesAsync();

    Task<Preference> GetPreferenceByIdAsync(Guid id);

    Task<IEnumerable<Preference>> GetPreferencesByIdsAsync(List<Guid> ids);
}
