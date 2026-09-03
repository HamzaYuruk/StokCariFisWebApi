using CariWebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;


namespace CariWebApi.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserCompanyRole> UserCompanyRoles { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<ReceiptDetail> ReceiptDetails { get; set; }
    public DbSet<StockTrans> StockTrans { get; set; }
    public DbSet<ActTrans> ActTrans { get; set; }
    public DbSet<Role>  Roles { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm decimal alanlar için ortak precision/scale ayarı
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(decimal)))
        {
            property.SetPrecision(18);
            property.SetScale(2);
        }

        // Tüm foreign key'lerde cascade delete yerine restrict kullan
        foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        // veritabanı diagramlarında tanımladığımız unique kısıtlamaları
        modelBuilder.Entity<Stock>()
            .HasIndex(s => new { s.CompanyId, s.Code })
            .IsUnique();

        modelBuilder.Entity<Account>()
            .HasIndex(a => new { a.CompanyId, a.Code })
            .IsUnique();

        modelBuilder.Entity<Receipt>()
            .HasIndex(r => new { r.CompanyId, r.ReceiptType, r.ReceiptNumber })
            .IsUnique();
    }

}