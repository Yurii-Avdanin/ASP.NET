using Microsoft.AspNetCore.Mvc;
using Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;
using Pcf.ReceivingFromPartner.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        private readonly IPreferencesService _preferencesService;

        public PreferencesController(IPreferencesService preferencesService)
        {
            _preferencesService = preferencesService;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            var preferences = await _preferencesService.GetPreferencesAsync();
            return Ok(preferences);
        }
    }
}