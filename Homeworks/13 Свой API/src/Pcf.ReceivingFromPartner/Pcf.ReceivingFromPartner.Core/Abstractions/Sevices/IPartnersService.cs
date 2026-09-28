using Pcf.ReceivingFromPartner.Core.Common;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;

public interface IPartnersService
{
    /// <summary>
    /// Получить список партнеров
    /// </summary>
    Task<Response<List<PartnerResponse>>> GetPartnersAsync();

    /// <summary>
    /// Получить информацию партнере
    /// </summary>
    /// <param name="id">Id партнера</param>
    Task<Response<PartnerResponse>> GetByIdPartnerAsync(Guid id);

    /// <summary>
    /// Установить лимит на промокоды для партнера
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<Response<Partner>> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request);

    /// <summary>
    /// Получить лимит на промокоды для партнера
    /// </summary>
    /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
    /// <param name="limitId">Id лимита партнера, например: <example>93f3a79d-e9f9-47e6-98bb-1f618db43230</example></param>
    Task<Response<PartnerPromoCodeLimitResponse>> GetPartnerLimitAsync(Guid id, Guid limitId);

    /// <summary>
    /// Отменить лимит на промокоды для партнера
    /// </summary>
    /// <param name="id">Id партнера, например: <example>0da65561-cf56-4942-bff2-22f50cf70d43</example></param>    
    Task<Response<Partner>> CancelPartnerPromoCodeLimitAsync(Guid id);

    /// <summary>
    /// Получить промокода партнера по id
    /// </summary>
    /// <returns></returns>    
    Task<Response<List<PromoCodeShortResponse>>> GetPartnerPromoCodesAsync(Guid id);

    /// <summary>
    /// Получить промокод партнера по id
    /// </summary>
    /// <returns></returns>    
    Task<Response<PromoCodeShortResponse>> GetPartnerPromoCodeAsync(Guid id, Guid promoCodeId);

    /// <summary>
    /// Создать промокод от партнера 
    /// </summary>
    /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
    /// <param name="request">Данные запроса/example></param>
    /// <returns></returns>
    Task<Guid> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid partnerId, ReceivingPromoCodeRequest request);
}
