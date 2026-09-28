using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class PreferenceQueries
{
    private readonly IPreferencesGatewayGrpc _preferencesGatewayGrpc;

    public PreferenceQueries(IPreferencesGatewayGrpc preferencesGatewayGrpc)
    {
        _preferencesGatewayGrpc = preferencesGatewayGrpc;
    }

    /// <summary>
    /// Получить список предпочтений (из внешнего сервиса).
    /// </summary>
    public async Task<IEnumerable<Preference>> GetPreferences()
    {
        return await _preferencesGatewayGrpc.GetAllPreferencesAsync();
    }

    /// <summary>
    /// Получить предпочтение по id.
    /// </summary>
    public async Task<Preference?> GetPreferenceById(Guid id)
    {
        return await _preferencesGatewayGrpc.GetPreferenceByIdAsync(id);
    }
}
