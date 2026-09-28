using GreenDonut;
using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.GraphQL.DataLoaders;

public class CustomerFullNameDataLoader : BatchDataLoader<Guid, string>
{
    private readonly IDbContextFactory<DataContext> _factory;

    public CustomerFullNameDataLoader(
        IDbContextFactory<DataContext> factory,
        IBatchScheduler batchScheduler,
        DataLoaderOptions? options = null)
        : base(batchScheduler, options)
    {
        _factory = factory;
    }

    protected override async Task<IReadOnlyDictionary<Guid, string>> LoadBatchAsync(
        IReadOnlyList<Guid> keys,
        CancellationToken cancellationToken)
    {
        await using var db = await _factory.CreateDbContextAsync(cancellationToken);

        var rows = await db.Customers
            .Where(c => keys.Contains(c.Id))
            .Select(c => new
            {
                c.Id,
                FullName = ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim()
            })
            .ToListAsync(cancellationToken);

        return rows.ToDictionary(r => r.Id, r => r.FullName);
    }
}
