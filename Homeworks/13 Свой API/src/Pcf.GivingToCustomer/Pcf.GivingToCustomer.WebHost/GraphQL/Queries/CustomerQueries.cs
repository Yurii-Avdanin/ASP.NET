using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;
using System;
using System.Linq;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class CustomerQueries
{   
    /// <summary>
    /// Получить список клиентов (краткие данные).
    /// </summary>    
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers([Service] DataContext context)
    {        
        return context.Customers;
    }       
    
    /// <summary>
    /// Получить клиента по id.
    /// </summary>
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Customer> GetCustomerById(Guid id, [Service] DataContext context)
    {  
        return context.Customers.Where(c => c.Id == id);
    }   
}
