using Microsoft.EntityFrameworkCore;
using Pcf.ReferenceInfo.Core.Domain;

namespace Pcf.ReferenceInfo.DataAccess;

public class DataContext : DbContext
{
    public DbSet<Preference> Preferences { get; set; }

    public DataContext() 
    { 
    }

    public DataContext(DbContextOptions<DataContext> options) : base(options) 
    { 
    }
}
