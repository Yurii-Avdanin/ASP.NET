using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Pcf.ReferenceInfo.Core.Abstractions.Repositories;
using Pcf.ReferenceInfo.Core.Domain;
using Pcf.ReferenceInfo.DataAccess.Data;
using Pcf.ReferenceInfo.DataAccess.Repositories;

namespace Pcf.ReferenceInfo.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        #region *** Repositories ********************************************************
        services.AddScoped<IRepository<Preference>, EfRepository<Preference>>();
        services.AddScoped<IDbInitializer, EfDbInitializer>();
        #endregion --- Repositories -----------------------------------------------------

        #region *** DataContext NpSql ***************************************************
        var connectionStringDb = configuration.GetConnectionString("PromocodeFactoryReferenceDb");
        services.AddDbContext<DataContext>(dbContextOptions =>
        {           
            dbContextOptions.UseNpgsql(connectionStringDb);
            dbContextOptions.UseSnakeCaseNamingConvention();            
        });
        #endregion --- DataContext NpSql ------------------------------------------------

        #region *** Redis ***************************************************************
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");            
            options.InstanceName = "PcfReferences:";
        });
        #endregion --- Redis ------------------------------------------------------------

        #region *** Swagger *************************************************************
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Promocode Factory Reference Info API",
                Version = "1.0",                
                Description = "API для работы со справочником предпочтений 'Preference'.",         
            });
        });
        #endregion ... Swagger ----------------------------------------------------------
        
        return services;
    }

    public static IApplicationBuilder Infrastructure(this IApplicationBuilder app)
    {
        #region *** Swagger *************************************************************
        app.UseSwagger();
        app.UseSwaggerUI();
        #endregion ... Swagger ----------------------------------------------------------

        return app;
    }
}
