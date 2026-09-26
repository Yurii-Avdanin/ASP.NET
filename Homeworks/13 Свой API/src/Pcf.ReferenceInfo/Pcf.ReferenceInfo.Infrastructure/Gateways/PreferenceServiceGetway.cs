using Google.Protobuf.WellKnownTypes;
using gRpc.Preference.V1;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Pcf.ReferenceInfo.Core.Abstractions.Services;

namespace Pcf.ReferenceInfo.Infrastructure.Gateways;

public class PreferenceServiceGetway : GrpcPreferenceService.GrpcPreferenceServiceBase
{
    private readonly ILogger<PreferenceServiceGetway> _logger;
    private readonly ICachedPreferenceService _cachedPreferenceService;

    public PreferenceServiceGetway(
        ILogger<PreferenceServiceGetway> logger,
        ICachedPreferenceService cachedPreferenceService)
    {
        _logger = logger;
        _cachedPreferenceService = cachedPreferenceService;
    }

    public override async Task<ResponseGrpcPreferenceRange> GetAll(Empty request, ServerCallContext context)
    {
        var listPreference = await _cachedPreferenceService.GetAllAsync();

        var response = new ResponseGrpcPreferenceRange();
        response.Preferences.AddRange(
            listPreference.Select(p => new PreferenceGrpc
            {
                Id = p.Id.ToString(),
                Name = p.Name
            }));


        return response;
    }

    public override async Task<ResponseGrpcPreference> GetById(
        RequestGrpcById request,
        ServerCallContext context)   {


        Guid id = Guid.Parse(request.Id);
        var preference = await _cachedPreferenceService.GetByIdAsync(id);

        var response = new ResponseGrpcPreference
        {
            Preference =  new PreferenceGrpc
                {
                    Id = preference.Id.ToString(),
                    Name = preference.Name
                }            
        };

        return response;
    }

    public override async Task<ResponseGrpcPreferenceRange> GetByRange(
        RequestGrpcByRange request,
        ServerCallContext context)
    {
        if (request.Ids.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "List Ids must not be empty"));
        }

        var ids = request.Ids.Select(Guid.Parse).ToList();                                   
        var listPreference = await _cachedPreferenceService.GetRangeByIdsAsync(ids);

        var response = new ResponseGrpcPreferenceRange
        {
            Preferences =
            {
                listPreference.Select(p => new PreferenceGrpc
                {
                    Id = p.Id.ToString(),
                    Name = p.Name
                })
            }
        };

        return response;
    }
}
