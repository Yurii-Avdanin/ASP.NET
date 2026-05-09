using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Mapping;
using PromoCodeFactory.WebHost.Models.Preferences;

namespace PromoCodeFactory.WebHost.Controllers;

/// <summary>
/// Предпочтения
/// </summary>
public class PreferencesController : BaseController
{
    private readonly IRepository<Preference> _preferenceEfRepository;

    public PreferencesController(IRepository<Preference> preferenceEfRepository)
    {
        _preferenceEfRepository = preferenceEfRepository;
    }
    /// <summary>
    /// Получить все доступные предпочтения
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PreferenceShortResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PreferenceShortResponse>>> Get(CancellationToken ct)
    {
        var preferences = await _preferenceEfRepository.GetAll(true, ct);        
        return Ok(preferences.Select(PreferencesMapper.ToPreferenceShortResponse));
    }
}
