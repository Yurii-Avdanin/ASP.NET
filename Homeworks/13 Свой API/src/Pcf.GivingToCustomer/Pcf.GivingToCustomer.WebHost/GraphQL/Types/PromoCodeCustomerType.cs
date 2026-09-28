using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.WebHost.GraphQL.DataLoaders;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Types;

public class PromoCodeCustomerType : ObjectType<PromoCodeCustomer>
{
    protected override void Configure(IObjectTypeDescriptor<PromoCodeCustomer> descriptor)
    {
        descriptor.Field(x => x.PromoCodeId).Type<NonNullType<IdType>>();
        descriptor.Field(x => x.CustomerId).Type<NonNullType<IdType>>();

        descriptor.Field(x => x.Customer)
            .Type<CustomerType>()
            .Resolve(async ctx =>
            {
                var link = ctx.Parent<PromoCodeCustomer>();
                var loader = ctx.DataLoader<CustomerByIdDataLoader>();
                return await loader.LoadAsync(link.CustomerId, ctx.RequestAborted);
            });
    }
}