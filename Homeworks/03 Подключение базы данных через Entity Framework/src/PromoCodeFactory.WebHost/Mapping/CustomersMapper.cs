using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models.Customers;

namespace PromoCodeFactory.WebHost.Mapping;

public static class CustomersMapper
{
    public static Customer ToCustomer(CustomerCreateRequest request, ICollection<Preference> preferences)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Preferences = preferences
        };
    }

    public static CustomerShortResponse ToCustomerShortResponse(Customer customer)
    {
        var preferenceShortResponse = customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList();

        return new CustomerShortResponse(
            Id: customer.Id,
            FirstName: customer.FirstName,
            LastName: customer.LastName,
            Email: customer.Email,
            Preferences: preferenceShortResponse);             
    }

    public static CustomerResponse ToCustomerResponse(Customer customer, IReadOnlyCollection<PromoCode> promoCodes)
    {
        var preferenceShortResponse = customer.Preferences.Select(PreferencesMapper.ToPreferenceShortResponse).ToList();
        var customerPromoCodeResponse = promoCodes.Select(p =>
        {
            var customerPromoCode = customer.CustomerPromoCodes.Single(e => e.PromoCodeId == p.Id);
            return PromoCodesMapper.ToCustomerPromoCodeResponse(p, customerPromoCode);
        }).ToList();

        return new CustomerResponse(
            Id: customer.Id,
            FirstName: customer.FirstName,
            LastName: customer.LastName,
            Email: customer.Email,
            Preferences: preferenceShortResponse,
            PromoCodes: customerPromoCodeResponse);

    }    
}
