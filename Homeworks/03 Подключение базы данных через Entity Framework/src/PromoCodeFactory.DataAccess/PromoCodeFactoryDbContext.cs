using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    //TODO: Добавить DbSet сущности
    #region Administration
    public DbSet<Role> Role { get; set; }
    public DbSet<Employee> Employee { get; set; }
    #endregion

    #region PromoCodeManagement
    public DbSet<Customer> Customer { get; set; }
    public DbSet<CustomerPromoCode> CustomerPromoCode { get; set; }
    public DbSet<Preference> Preference { get; set; }
    public DbSet<PromoCode> PromoCode { get; set; }
    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //TODO: Добавить маппинг моделей
        modelBuilder.Entity<Role>(RoleConfigure);
        modelBuilder.Entity<Employee>(EmployeeConfigure);

        modelBuilder.Entity<Customer>(CustomerConfigure);
        modelBuilder.Entity<Preference>(PreferenceConfigure);
        modelBuilder.Entity<PromoCode>(PromoCodeConfigure);

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Описание ограничений сущности "Role"
    /// </summary>
    /// <param name="entity"></param>
    private void RoleConfigure(EntityTypeBuilder<Role> entity)
    {
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Description).HasMaxLength(500);
    }

    /// <summary>
    /// Описание ограничений сущности "Employee"
    /// </summary>
    /// <param name="entity"></param>
    private void EmployeeConfigure(EntityTypeBuilder<Employee> entity)
    {
        entity.Property(e => e.FirstName).HasMaxLength(50);
        entity.Property(e => e.LastName).HasMaxLength(50);
        entity.Property(e => e.Email).HasMaxLength(256);
    }


    /// <summary>
    /// Описание ограничений сущности "Customer"
    /// </summary>
    /// <param name="entity"></param>
    private void CustomerConfigure(EntityTypeBuilder<Customer> entity)
    {
        entity.Property(e => e.FirstName).HasMaxLength(50);
        entity.Property(e => e.LastName).HasMaxLength(50);
        entity.Property(e => e.Email).HasMaxLength(256);
    }

    /// <summary>
    /// Описание ограничений сущности "Preference"
    /// </summary>
    /// <param name="entity"></param>
    private void PreferenceConfigure(EntityTypeBuilder<Preference> entity)
    {
        entity.Property(e => e.Name).HasMaxLength(100);        
    }

    /// <summary>
    /// Описание ограничений сущности "PromoCode"
    /// </summary>
    /// <param name="entity"></param>
    private void PromoCodeConfigure(EntityTypeBuilder<PromoCode> entity)
    {
        entity.Property(e => e.Code).HasMaxLength(101);
        entity.Property(e => e.ServiceInfo).HasMaxLength(256);
        entity.Property(e => e.PartnerName).HasMaxLength(100);      
    }
}
