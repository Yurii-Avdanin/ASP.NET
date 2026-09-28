using Google.Protobuf.WellKnownTypes;
using gRpc.Preference.V1;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Integration;

public class PreferencesGetewayGrpc : IPreferencesGetewayGrpc
{
    private readonly GrpcPreferenceService.GrpcPreferenceServiceClient _client_gRPC;

    public PreferencesGetewayGrpc(
        GrpcPreferenceService.GrpcPreferenceServiceClient client_gRPC)
    {
        _client_gRPC = client_gRPC;
    }

    public async Task<IEnumerable<Preference>> GetAllAsync()
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

    public async Task<Preference> GetByIdAsync(Guid id)
    {
        var request = new RequestGrpcById { Id = id.ToString() };

        var responseGrpc = await _client_gRPC.GetByIdAsync(request);

        var preference = new Preference
        {
            Id = Guid.Parse(responseGrpc.Preference.Id),
            Name = responseGrpc.Preference.Name,
        };

        return preference;
    }
}
