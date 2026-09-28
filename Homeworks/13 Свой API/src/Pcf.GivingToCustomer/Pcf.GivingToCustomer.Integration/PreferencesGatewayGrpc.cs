using Google.Protobuf.WellKnownTypes;
using gRpc.Preference.V1;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Integration;

public class PreferencesGatewayGrpc : IPreferencesGatewayGrpc
{
    private readonly GrpcPreferenceService.GrpcPreferenceServiceClient _client_gRPC;

    public PreferencesGatewayGrpc(
        GrpcPreferenceService.GrpcPreferenceServiceClient client_gRPC)
    {
        _client_gRPC = client_gRPC;
    }

    public async Task<IEnumerable<Preference>> GetAllPreferencesAsync()
    {
        var responseGrpc = await _client_gRPC.GetAllAsync(new Empty());

        var preferences = responseGrpc.Preferences
            .Select(p => new Preference
            {
                Id = Guid.Parse(p.Id),
                Name = p.Name,
            });

        return preferences;
    }

    public async Task<Preference?> GetPreferenceByIdAsync(Guid id)
    {
        var request = new RequestGrpcById { Id = id.ToString() };

        var responseGrpc = await _client_gRPC.GetByIdAsync(request);

        if (responseGrpc.Preference is null) 
            return null;

        var preference = new Preference
        {
            Id = Guid.Parse(responseGrpc.Preference.Id),
            Name = responseGrpc.Preference.Name,
        };

        return preference;
    }

    public async Task<IEnumerable<Preference>> GetPreferencesByIdsAsync(List<Guid> ids)
    {
        var request = new RequestGrpcByRange{ 
            Ids = { ids.Select(id => id.ToString()) }
        };

        var responseGrpc = await _client_gRPC.GetByRangeAsync(request);

        var preferences = responseGrpc.Preferences
            .Select(p => new Preference
            {
                Id = Guid.Parse(p.Id),
                Name = p.Name,
            });

        return preferences;
    }
}

