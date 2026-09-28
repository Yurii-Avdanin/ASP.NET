using Microsoft.AspNetCore.Mvc;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Integration;
using Pcf.GivingToCustomer.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController : ControllerBase
    {
        //private readonly IPreferencesGateway _preferencesGateway;
        private readonly IPreferencesGatewayGrpc _preferencesGatewayGrpc;

        public PreferencesController(//IPreferencesGateway preferencesGateway,
            IPreferencesGatewayGrpc preferencesGetewayGrpc)
        {
            //_preferencesGateway = preferencesGateway;
            _preferencesGatewayGrpc = preferencesGetewayGrpc;
        }

        /// <summary>
        /// Получить список предпочтений
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<PreferenceResponse>>> GetPreferencesAsync()
        {
            //var preferences = await _preferencesGateway.GetAllPreferencesAsync();
            var preferences = await _preferencesGatewayGrpc.GetAllPreferencesAsync();

            var response = preferences.Select(x => new PreferenceResponse()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();

            return Ok(response);
        }
    }
}