using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Abstractions.Services;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.Core.Services;

public class PromoCodeService : IPromoCodeService
{
    private readonly IRepository<PromoCode> _promoCodesRepository;    
    private readonly IRepository<Customer> _customersRepository;
    private readonly IPreferencesGateway _preferencesGateway;

    public PromoCodeService(
           IRepository<PromoCode> promoCodesRepository,           
           IRepository<Customer> customersRepository,
           IPreferencesGateway preferencesGateway)
    {
        _promoCodesRepository = promoCodesRepository;        
        _customersRepository = customersRepository;
        _preferencesGateway = preferencesGateway;
    }

    public async Task GivePromoCodesToCustomersWithPreferenceAsync(PromoCodeRequestDto request)           
    {
        var preference = await _preferencesGateway.GetPreferenceByIdAsync(request.PreferenceId);
        if (preference == null)
            throw new InvalidOperationException("Предпочтение не найдено");

        var customers = await _customersRepository
            .GetWhere(d => d.Preferences.Any(x => x.Preference.Id == preference.Id));

        var entity = new PromoCode
        {
            Id = request.PromoCodeId,
            Code = request.PromoCode,
            ServiceInfo = request.ServiceInfo,
            PartnerId = request.PartnerId,
            BeginDate = request.BeginDate,
            EndDate = request.EndDate,
            Preference = preference,
            PreferenceId = preference.Id,
            Customers = new List<PromoCodeCustomer>()
        };

        foreach (var customer in customers)
        {
            entity.Customers.Add(new PromoCodeCustomer
            {
                CustomerId = customer.Id,
                Customer = customer,
                PromoCodeId = entity.Id,
                PromoCode = entity
            });
        }

        await _promoCodesRepository.AddAsync(entity);
    }
}
