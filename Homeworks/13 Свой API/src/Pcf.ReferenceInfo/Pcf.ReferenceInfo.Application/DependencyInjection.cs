using Microsoft.Extensions.DependencyInjection;
using Pcf.ReferenceInfo.Application.Services;
using Pcf.ReferenceInfo.Core.Abstractions.Services;

namespace Pcf.ReferenceInfo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {        
        #region *** Services ************************************************************
        services.AddScoped<ICachedPreferenceService, CachedPreferenceService>();
        #endregion ... Services ---------------------------------------------------------        

        return services;
    }    
}
