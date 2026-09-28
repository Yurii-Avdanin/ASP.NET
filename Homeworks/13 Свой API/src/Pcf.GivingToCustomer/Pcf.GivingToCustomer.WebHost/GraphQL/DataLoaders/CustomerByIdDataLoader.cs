using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.DataLoaders;

public class CustomerByIdDataLoader : BatchDataLoader<Guid, Customer>
{
    private readonly IDbContextFactory<DataContext> _dbContextFactory;

    public CustomerByIdDataLoader(
        IDbContextFactory<DataContext> dbContextFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task<IReadOnlyDictionary<Guid, Customer>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        return await db.Customers
            .Where(c => keys.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, cancellationToken);
    }
}