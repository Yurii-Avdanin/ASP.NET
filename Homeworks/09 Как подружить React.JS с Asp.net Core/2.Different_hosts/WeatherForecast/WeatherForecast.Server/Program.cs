
using WeatherForecast.Server;

string corsName = "MyAllowReactApp";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(builder.Configuration, corsName);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});

app.UseCors(corsName);

app.UseAuthorization();

app.MapControllers();

app.Run();
