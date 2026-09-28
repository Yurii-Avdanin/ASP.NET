using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Domain;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Types;

public class PreferenceType : ObjectType<Preference>
{
    protected override void Configure(IObjectTypeDescriptor<Preference> descriptor)
    {
        descriptor.Field(p => p.Id).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.Name).Type<NonNullType<StringType>>();
    }
}