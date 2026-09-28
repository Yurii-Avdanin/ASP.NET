using Microsoft.Extensions.Caching.Distributed;
using Pcf.ReferenceInfo.Core.Abstractions.Repositories;
using Pcf.ReferenceInfo.Core.Abstractions.Services;
using Pcf.ReferenceInfo.Core.Domain;
using System.Text.Json;

namespace Pcf.ReferenceInfo.Application.Services;

public class CachedPreferenceService : ICachedPreferenceService
{
    private const string AllPreferencesKey = "preferences:all";
    private static string PreferenceKey(Guid id) => $"preferences:{id}";

    private static readonly DistributedCacheEntryOptions CacheOptions = 
        new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

    private readonly IRepository<Preference> _repository;
    private readonly IDistributedCache _cache;

    public CachedPreferenceService(IRepository<Preference> repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<Preference>> GetAllAsync()
    {
        var cached = await _cache.GetStringAsync(AllPreferencesKey);
        if (!string.IsNullOrEmpty(cached))       
            return JsonSerializer.Deserialize<List<Preference>>(cached)!;
            
        var preferences = (await _repository.GetAllAsync()).ToList();

        await _cache.SetStringAsync(AllPreferencesKey, JsonSerializer.Serialize(preferences), CacheOptions);

        return preferences;
    }

    public async Task<Preference> GetByIdAsync(Guid id)
    {
        var key = PreferenceKey(id);
        var cached = await _cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<Preference>(cached)!;

        var preference = await _repository.GetByIdAsync(id);
        if (preference == null) 
            return null;

        await _cache.SetStringAsync(key, JsonSerializer.Serialize(preference), CacheOptions);
        return preference;
    }

    public async Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
            return Enumerable.Empty<Preference>();

        var result = new List<Preference>();
        var missingIds = new List<Guid>();

        foreach (var id in ids)
        {
            var cached = await _cache.GetStringAsync(PreferenceKey(id));
            if (!string.IsNullOrEmpty(cached))
                result.Add(JsonSerializer.Deserialize<Preference>(cached)!);
            else
                missingIds.Add(id);
        }

        if (missingIds.Count > 0)
        {
            var fromDb = (await _repository.GetRangeByIdsAsync(missingIds)).ToList();
            foreach (var p in fromDb)
            {
                await _cache.SetStringAsync(PreferenceKey(p.Id), JsonSerializer.Serialize(p), CacheOptions);
                result.Add(p);
            }
        }

        return result;
    }
}