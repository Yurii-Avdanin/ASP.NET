using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Abstractions.Gateways;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Types;

public class PromoCodeType : ObjectType<PromoCode>
{
    protected override void Configure(IObjectTypeDescriptor<PromoCode> descriptor)
    {
        descriptor.Field(p => p.Id).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.Code).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.ServiceInfo).Type<StringType>();
        descriptor.Field(p => p.BeginDate).Type<NonNullType<DateTimeType>>();
        descriptor.Field(p => p.EndDate).Type<NonNullType<DateTimeType>>();
        descriptor.Field(p => p.PartnerId).Type<NonNullType<IdType>>();

        descriptor.Field("preference")
            .Type<PreferenceType>()
            .Resolve(async ctx =>
            {
                var promoCode = ctx.Parent<PromoCode>();
                var gateway = ctx.Service<IPreferencesGatewayGrpc>();
                return await gateway.GetPreferenceByIdAsync(promoCode.PreferenceId);
            });
        
        descriptor.Field(p => p.Customers)
            .Type<ListType<PromoCodeCustomerType>>()
            .Name("customers");
    }
}