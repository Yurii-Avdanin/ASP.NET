using Pcf.ReceivingFromPartner.Application.Mappers;
using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;
using Pcf.ReceivingFromPartner.Core.Common;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;

namespace Pcf.ReceivingFromPartner.Application.Sevices;

public class PartnersService : IPartnersService
{
    private readonly IRepository<Partner> _partnersRepository;    
    private readonly IPromoCodeMessageGateway _promoCodeMessageGateway;

    private readonly INotificationGateway _notificationGateway;
    //private readonly IPreferencesGatewayHttp _preferencesGateway;
    private readonly IPreferencesGetewayGrpc _preferencesGatewayGrpc;

    public PartnersService(
        IRepository<Partner> partnersRepository,        
        IPromoCodeMessageGateway promoCodeMessageGateway,
        INotificationGateway notificationGateway,
        //IPreferencesGatewayHttp preferencesGateway,
        IPreferencesGetewayGrpc preferencesGatewayGrpc)
    {
        _partnersRepository = partnersRepository;        
        _promoCodeMessageGateway = promoCodeMessageGateway;

        _notificationGateway = notificationGateway;
        //_preferencesGateway = preferencesGateway;
        _preferencesGatewayGrpc = preferencesGatewayGrpc;
    }

    /// <summary>
    /// Получить список партнеров
    /// </summary>
    public async Task<Response<List<PartnerResponse>>> GetPartnersAsync()
    {
        var partners = await _partnersRepository.GetAllAsync();
        return Response<List<PartnerResponse>>.Success(partners.ToPartnerResponse().ToList());
    }

    /// <summary>
    /// Получить информацию партнере
    /// </summary>
    /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
    public async Task<Response<PartnerResponse>> GetByIdPartnerAsync(Guid id)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);
        if (partner == null)
            return Response<PartnerResponse>.Failure("Партнер не найден.");

        return Response<PartnerResponse>.Success(partner.ToPartnerResponse());
    }

    /// <summary>
    /// Установить лимит на промокоды для партнера
    /// </summary>
    public async Task<Response<Partner>> SetPartnerPromoCodeLimitAsync(Guid id, SetPartnerPromoCodeLimitRequest request)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);

        if (partner == null)        
            return Response<Partner>.Failure("Партнер не найден.");

        //Если партнер заблокирован, то нужно выдать исключение
        if (!partner.IsActive)
            return Response<Partner>.Failure("Данный партнер не активен.");        

        //Установка лимита партнеру
        var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
            !x.CancelDate.HasValue);

        if (activeLimit != null)
        {
            //Если партнеру выставляется лимит, то мы 
            //должны обнулить количество промокодов, которые партнер выдал, если лимит закончился, 
            //то количество не обнуляется
            partner.NumberIssuedPromoCodes = 0;

            //При установке лимита нужно отключить предыдущий лимит
            activeLimit.CancelDate = DateTime.Now;
        }

        if (request.Limit <= 0)
            return Response<Partner>.Failure("Лимит должен быть больше 0.");

        var newLimit = new PartnerPromoCodeLimit()
        {
            Limit = request.Limit,
            Partner = partner,
            PartnerId = partner.Id,
            CreateDate = DateTime.Now,
            EndDate = request.EndDate
        };

        partner.PartnerLimits.Add(newLimit);

        await _partnersRepository.UpdateAsync(partner);       

        await _notificationGateway.SendNotificationToPartnerAsync(partner.Id, "Вам установлен лимит на отправку промокодов...");

        return Response<Partner>.Success(partner);
    }

    /// <summary>
    /// Получить лимит на промокоды для партнера
    /// </summary>
    /// <param name="id">Id партнера, например: <example>20d2d612-db93-4ed5-86b1-ff2413bca655</example></param>
    /// <param name="limitId">Id лимита партнера, например: <example>93f3a79d-e9f9-47e6-98bb-1f618db43230</example></param>
    public async Task<Response<PartnerPromoCodeLimitResponse>> GetPartnerLimitAsync(Guid id, Guid limitId)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);

        if (partner == null)
            return Response<PartnerPromoCodeLimitResponse>.Failure("Партнер не найден.");

        var limit = partner.PartnerLimits
            .FirstOrDefault(x => x.Id == limitId);

        if (limit == null)
            return Response<PartnerPromoCodeLimitResponse>.Failure("Лимит не найден.");

        return Response<PartnerPromoCodeLimitResponse>.Success(limit.ToPartnerPromoCodeLimitResponse());
    }

    /// <summary>
    /// Отменить лимит на промокоды для партнера
    /// </summary>
    /// <param name="id">Id партнера, например: <example>0da65561-cf56-4942-bff2-22f50cf70d43</example></param>    
    public async Task<Response<Partner>> CancelPartnerPromoCodeLimitAsync(Guid id)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);

        if (partner == null)
            return Response<Partner>.Failure("Партнер не найден.");

        //Если партнер заблокирован, то нужно выдать исключение
        if (!partner.IsActive)
            return Response<Partner>.Failure("Данный партнер не активен");

        //Отключение лимита
        var activeLimit = partner.PartnerLimits.FirstOrDefault(x =>
            !x.CancelDate.HasValue);

        if (activeLimit != null)
        {
            activeLimit.CancelDate = DateTime.Now;
        }

        await _partnersRepository.UpdateAsync(partner);

        //Отправляем уведомление
        await _notificationGateway
            .SendNotificationToPartnerAsync(partner.Id, "Ваш лимит на отправку промокодов отменен...");

        return Response<Partner>.Success(partner);
    }

    /// <summary>
    /// Получить промокода партнера по id
    /// </summary>
    /// <returns></returns>    
    public async Task<Response<List<PromoCodeShortResponse>>> GetPartnerPromoCodesAsync(Guid id)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);
        if (partner == null)
            return Response<List<PromoCodeShortResponse>>.Failure("Партнер не найден.");

        return Response<List<PromoCodeShortResponse>>.Success(partner.PromoCodes.ToPromoCodeShortResponse().ToList());
    }

    /// <summary>
    /// Получить промокод партнера по id
    /// </summary>
    /// <returns></returns>    
    public async Task<Response<PromoCodeShortResponse>> GetPartnerPromoCodeAsync(Guid id, Guid promoCodeId)
    {
        var partner = await _partnersRepository.GetByIdAsync(id);
        if (partner == null)
            return Response<PromoCodeShortResponse>.Failure("Партнер не найден.");

        var promoCode = partner.PromoCodes.FirstOrDefault(x => x.Id == promoCodeId);

        if (promoCode == null)
            return Response<PromoCodeShortResponse>.Failure("промокод не найден у партнера.");

        return Response<PromoCodeShortResponse>.Success(promoCode.ToPromoCodeShortResponse());
    }
  
    /// <summary>
    /// 
    /// </summary>
    /// <param name="partnerId"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Guid> ReceivePromoCodeFromPartnerWithPreferenceAsync(Guid partnerId, ReceivingPromoCodeRequest request)
    {
        var partner = await _partnersRepository.GetByIdAsync(partnerId);

        if (partner == null)
            throw new InvalidOperationException("Партнер не найден");

        var activeLimit = partner.PartnerLimits.FirstOrDefault(x
            => !x.CancelDate.HasValue && x.EndDate > DateTime.Now);

        if (activeLimit == null)
            throw new InvalidOperationException("Нет доступного лимита на предоставление промокодов");

        if (partner.NumberIssuedPromoCodes + 1 > activeLimit.Limit)
            throw new InvalidOperationException("Лимит на выдачу промокодов превышен");

        if (partner.PromoCodes.Any(x => x.Code == request.PromoCode))
            throw new InvalidOperationException("Данный промокод уже был выдан ранее");

        //Получаем предпочтение        
        //var preference = await _preferencesGateway.GetByIdAsync(request.PreferenceId);
        var preference = await _preferencesGatewayGrpc.GetByIdAsync(request.PreferenceId);

        if (preference == null)
            throw new InvalidOperationException("Предпочтение не найдено");

        PromoCode promoCode = new PromoCode
        {            
            PartnerId = partner.Id,
            Partner = partner,
            Code = request.PromoCode,
            ServiceInfo = request.ServiceInfo,
            BeginDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            Preference = preference,
            PreferenceId = preference.Id,
            PartnerManagerId = request.PartnerManagerId
        };

        partner.PromoCodes.Add(promoCode);
        partner.NumberIssuedPromoCodes++;

        await _partnersRepository.UpdateAsync(partner);

        // Единое событие вместо двух HTTP-вызовов
        await _promoCodeMessageGateway.PublishPromoCodeAsync(promoCode.ToPromoCodeMessage());

        return promoCode.Id;
    }
}
