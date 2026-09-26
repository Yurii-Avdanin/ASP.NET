using Microsoft.AspNetCore.Mvc;
using Pcf.ReferenceInfo.Core.Abstractions.Services;
using Pcf.ReferenceInfo.WebHost.Models;
using System.ComponentModel.Design;

namespace Pcf.ReferenceInfo.WebHost.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PreferencesController : ControllerBase
{
    private readonly ICachedPreferenceService _preferenceService;

    public PreferencesController(ICachedPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
    {
        var preferences = await _preferenceService.GetAllAsync();

        var response = preferences.Select(x => new PreferenceResponse
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PreferenceResponse>> GetPreferenceAsync(Guid id)
    {
        var preference = await _preferenceService.GetByIdAsync(id);
        if (preference == null) return NotFound();

        return Ok(new PreferenceResponse { Id = preference.Id, Name = preference.Name });
    }

    [HttpPost("range")]
    public async Task<ActionResult<List<PreferenceResponse>>> GetRangeAsync([FromBody] List<Guid> ids)
    {
        var preferences = await _preferenceService.GetRangeByIdsAsync(ids);

        var response = preferences.Select(x => new PreferenceResponse
        {
            Id = x.Id,
            Name = x.Name
        }).ToList();

        return Ok(response);
    }
}
