using gRpc.Preference.V1;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using System;
using System.Net.Http;

namespace Pcf.GivingToCustomer.Integration;

public static class DependencyInjection
{
    public static IServiceCollection AddGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        var gRPC_Url = configuration["Grpc:ReferencesInfoUrl"] ?? "unknown";

        // Регистрируем gRPC-клиент через фабрику
        services.AddGrpcClient<GrpcPreferenceService.GrpcPreferenceServiceClient>((sp, options) =>
        {
            options.Address = new Uri(gRPC_Url);

            // Для разработки: отключаем TLS, если адрес http://
            if (options.Address.Scheme == Uri.UriSchemeHttp)
            {
                options.ChannelOptionsActions.Add(channelOptions =>
                {
                    channelOptions.Credentials = ChannelCredentials.Insecure;
                });
            }
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
        {            
            MaxConnectionsPerServer = 10,
            PooledConnectionLifetime = TimeSpan.FromMinutes(10),
            PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
            EnableMultipleHttp2Connections = true
        });

        services.AddScoped<IPreferencesGatewayGrpc, PreferencesGatewayGrpc>();        

        return services;
    }
}

