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

public class CustomerPreferenceIdsDataLoader : BatchDataLoader<Guid, List<Guid>>
{
    private readonly IDbContextFactory<DataContext> _dbFactory;

    public CustomerPreferenceIdsDataLoader(
        IDbContextFactory<DataContext> dbFactory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _dbFactory = dbFactory;
    }

    protected override async Task<IReadOnlyDictionary<Guid, List<Guid>>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

        var rows = await db.Set<CustomerPreference>()
            .Where(cp => keys.Contains(cp.CustomerId))
            .Select(cp => new { cp.CustomerId, cp.PreferenceId })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(r => r.CustomerId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => x.PreferenceId).ToList());
    }
}
