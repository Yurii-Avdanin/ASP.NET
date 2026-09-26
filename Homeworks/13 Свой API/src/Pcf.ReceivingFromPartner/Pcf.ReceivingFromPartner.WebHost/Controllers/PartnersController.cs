using Microsoft.AspNetCore.Mvc;
using Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;
using Pcf.ReceivingFromPartner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers
{
    /// <summary>
    /// Партнеры
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnersService _partnersService;
        
        public PartnersController(IPartnersService partnersService)
        {            
            _partnersService = partnersService;
        }

        /// <summary>
        /// Получить список партнеров
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<PartnerResponse>>> GetPartnersAsync()
        {
            var response = await _partnersService.GetPartnersAsync();            
            return Ok(response.Data);
        }

        /// <summary>
        /// Получить информацию партнере
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<List<PartnerResponse>>> GetByIdPartnersAsync(Guid id)
        {
            var response = await _partnersService.GetByIdPartnerAsync(id);
            if (response.Data == null)            
                return NotFound();

            return Ok(response.Data);
        }

        /// <summary>
        /// Установить лимит на промокоды для партнера
        /// </summary>
        [HttpPost("{id:guid}/limits")]
        public async Task<IActionResult> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
        {
            var response = await _partnersService.SetPartnerPromoCodeLimitAsync(id, request);
            var partner = response.Data;
            
            if (partner == null)
                return NotFound(response.ErrMsg);
            
            if (!response.IsSuccess)
                return BadRequest(response.ErrMsg);
                        
            return CreatedAtAction(
                nameof(SetPartnerPromoCodeLimitAsync), 
                new { id = partner.Id, limitId = partner.PartnerLimits.LastOrDefault()!.Id }, 
                null);
        }

        /// <summary>
        /// Получить лимит на промокоды для партнера
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        /// <param name="limitId">Id лимита партнера, например: <example>93f3a79d-e9f9-47e6-98bb-1f618db43230</example></param>
        [HttpGet("{id:guid}/limits/{limitId:guid}")]
        public async Task<ActionResult<PartnerPromoCodeLimitResponse>> GetPartnerLimitAsync(Guid id, Guid limitId)
        {
            var response = await _partnersService.GetPartnerLimitAsync(id, limitId);
            if (!response.IsSuccess)
                return NotFound(response.ErrMsg);

            return Ok(response.Data);
        }

        /// <summary>
        /// Отменить лимит на промокоды для партнера
        /// </summary>
        /// <param name="id">Id партнера, например: <example>0da65561-cf56-4942-bff2-22f50cf70d43</example></param>
        [HttpPost("{id:guid}/canceledLimits")]
        public async Task<IActionResult> CancelPartnerPromoCodeLimitAsync(Guid id)
        {
            var response = await _partnersService.CancelPartnerPromoCodeLimitAsync(id);            
            if (response.Data == null)
                return NotFound(response.ErrMsg);
            
            if (!response.IsSuccess)
                return BadRequest(response.ErrMsg);

            return NoContent();
        }

        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}/promocodes")]
        public async Task<IActionResult> GetPartnerPromoCodesAsync(Guid id)
        {
            var response = await _partnersService.CancelPartnerPromoCodeLimitAsync(id);
            if (response.Data == null)
                return NotFound(response.ErrMsg);
           
            return Ok(response);
        }

        /// <summary>
        /// Получить промокод партнера по id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}/promocodes/{promoCodeId:guid}")]
        public async Task<IActionResult> GetPartnerPromoCodeAsync(Guid id, Guid promoCodeId)
        {
            var response = await _partnersService.CancelPartnerPromoCodeLimitAsync(id);
            if (!response.IsSuccess)
                return NotFound(response.ErrMsg);

            return Ok(response);
        }

        /// <summary>
        /// Создать промокод от партнера 
        /// </summary>
        /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
        /// <param name="request">Данные запроса/example></param>
        /// <returns></returns>
        [HttpPost("{id:guid}/promocodes")]
        public async Task<IActionResult> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid id,
            ReceivingPromoCodeRequest request)
        {
            try
            {
                var promoCodeId = await _partnersService
                    .ReceivePromoCodeFromPartnerWithPreferenceAsync(id, request);

                return CreatedAtAction(
                    nameof(GetPartnerPromoCodeAsync),
                    new { id = id, promoCodeId = promoCodeId }, 
                    null);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}