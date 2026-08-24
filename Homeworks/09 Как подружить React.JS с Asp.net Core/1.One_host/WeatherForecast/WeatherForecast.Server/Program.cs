using WeatherForecast.Server;

string corsName = "MyAllowReactApp";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(builder.Configuration, corsName);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";    
});

//..........
var app = builder.Build();

app.UseSpaStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseCors(corsName);

app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
app.UseSpa(spa =>
{
    spa.Options.SourcePath = "../weatherforecast.client";
});

app.Run();
