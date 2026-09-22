using Pcf.ReferenceInfo.Core.Domain;

namespace Pcf.ReferenceInfo.Core.Abstractions.Services;

public interface ICachedPreferenceService
{
    Task<IEnumerable<Preference>> GetAllAsync();

    Task<Preference> GetByIdAsync(Guid id);

    Task<IEnumerable<Preference>> GetRangeByIdsAsync(List<Guid> ids);
}
