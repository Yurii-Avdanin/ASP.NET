using Pcf.ReceivingFromPartner.Core.Abstractions.Gateways;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Abstractions.Sevices;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.Core.DTOs;
using Pcf.ReceivingFromPartner.Core.Mappers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Application.Sevices;

public class PromoCodeFromPartnerService : IPromoCodeFromPartnerService
{
    private readonly IRepository<Partner> _partnersRepository;
    private readonly IRepository<Preference> _preferencesRepository;
    private readonly IPromoCodeMessageGateway _promoCodeMessageGateway;

    public PromoCodeFromPartnerService(
        IRepository<Partner> partnersRepository,
        IRepository<Preference> preferencesRepository,
        IPromoCodeMessageGateway promoCodeMessageGateway)
    {
        _partnersRepository = partnersRepository;
        _preferencesRepository = preferencesRepository;
        _promoCodeMessageGateway = promoCodeMessageGateway;
    }

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
        var preference = await _preferencesRepository.GetByIdAsync(request.PreferenceId);
        
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
