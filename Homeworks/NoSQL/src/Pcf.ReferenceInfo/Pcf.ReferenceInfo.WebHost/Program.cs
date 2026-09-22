using Pcf.ReferenceInfo.Application;
using Pcf.ReferenceInfo.DataAccess;
using Pcf.ReferenceInfo.DataAccess.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.Infrastructure();

//app.MapGet("/", () => "Hello World!");

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer.InitializeDb();
}

app.Run();
