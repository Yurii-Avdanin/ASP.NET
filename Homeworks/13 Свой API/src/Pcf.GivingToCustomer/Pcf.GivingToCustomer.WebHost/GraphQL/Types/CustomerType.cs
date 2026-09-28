using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.GraphQL.DataLoaders;
using System.Linq;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Types;

public class CustomerType : ObjectType<Customer>
{
    protected override void Configure(IObjectTypeDescriptor<Customer> descriptor)
    {
        descriptor.Field(c => c.Id).Type<NonNullType<IdType>>();
        descriptor.Field(c => c.FirstName).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.LastName).Type<NonNullType<StringType>>();        
        descriptor.Field(c => c.Email).Type<NonNullType<StringType>>();

        // Full name
        descriptor.Field("fullName")
            .Type<NonNullType<StringType>>()
            .Resolve(async ctx =>
            {
                var customer = ctx.Parent<Customer>();
                var loader = ctx.DataLoader<CustomerFullNameDataLoader>();
                var fullName = await loader.LoadAsync(customer.Id, ctx.RequestAborted);

                return fullName.Trim();                
            });

        // Предпочтения клиента
        descriptor.Field("preferences")
            .Type<ListType<PreferenceType>>()
            .Resolve(async ctx =>
            {
                var customer = ctx.Parent<Customer>();
                var loader = ctx.DataLoader<CustomerPreferenceIdsDataLoader>();
                var preferenceIds = await loader.LoadAsync(customer.Id, ctx.RequestAborted);
                
                if (preferenceIds is null || preferenceIds.Count == 0)
                    return Enumerable.Empty<Preference>();

                var gateway = ctx.Service<IPreferencesGatewayGrpc>();
                return await gateway.GetPreferencesByIdsAsync(preferenceIds);
            });        

        // Промокоды клиента
        descriptor.Field("promoCodes")
            .Type<ListType<PromoCodeType>>()
            .Resolve(async ctx =>
            {
                var customer = ctx.Parent<Customer>();
                var loader = ctx.DataLoader<CustomerPromoCodeIdsDataLoader>();                
                var promoCodeIds = await loader.LoadAsync(customer.Id, ctx.RequestAborted);

                if (promoCodeIds is null || promoCodeIds.Count == 0) 
                    return Enumerable.Empty<PromoCode>();

                var repo = ctx.Service<IRepository<PromoCode>>();
                return await repo.GetRangeByIdsAsync(promoCodeIds);
            });
    }
}
