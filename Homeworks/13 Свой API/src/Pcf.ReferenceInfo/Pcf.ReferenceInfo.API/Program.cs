using Microsoft.AspNetCore.Server.Kestrel.Core;
using Pcf.ReferenceInfo.Application;
using Pcf.ReferenceInfo.Infrastructure;
using Pcf.ReferenceInfo.Infrastructure.Gateways;
using Pcf.ReferenceInfo.Infrastructure.Persistence.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

#region *** gRPC ****************************************************************        
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(options =>
{
    // REST (HTTP/1.1 и HTTP/2 поддерживаются, но для REST используем HTTP/1.1)
    options.ListenAnyIP(5002, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
    });
    // gRPC (только HTTP/2 без TLS)
    options.ListenAnyIP(6002, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});
#endregion ... gRPC -------------------------------------------------------------

var app = builder.Build();

app.Infrastructure();

app.MapGrpcService<PreferenceServiceGetway>();

//app.MapGet("/", () => "Hello World!");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.InitializeDb();
}

app.Run();
