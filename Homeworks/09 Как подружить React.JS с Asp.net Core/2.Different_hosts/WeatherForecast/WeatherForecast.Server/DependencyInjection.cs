namespace WeatherForecast.Server;

public static class DependencyInjection
{
    public static IServiceCollection AddCors(this IServiceCollection services,
        IConfiguration configuration, string corsName)
    {
        var origins = configuration.GetSection("CORS:Origins").Get<string[]>();
        if (origins == null || origins.Length == 0)
            origins = new[] { "*" };

        var headers = configuration.GetSection("CORS:Headers").Get<string[]>();
        if (headers == null || headers.Length == 0)
            headers = new[] { "Content-Type", "Authorization" };

        var methods = configuration.GetSection("CORS:Methods").Get<string[]>();
        if (methods == null || methods.Length == 0)
            methods = new[] { "GET", "POST", "PUT", "DELETE" };

        
        //Console.WriteLine($"CORS Origins: {string.Join(", ", origins ?? Array.Empty<string>())}");

        services.AddCors(options =>
        {
            options.AddPolicy(corsName,
                corsBuilder =>
                {
                    corsBuilder
                      .WithOrigins(origins)
                      .WithHeaders(headers)
                      .WithMethods(methods);
                });
        });

        return services;
    }

}