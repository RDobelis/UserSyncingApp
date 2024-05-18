using Microsoft.EntityFrameworkCore;
using MyApp.ServiceModel.Types;

namespace UserSyncingApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Geo> Geos { get; set; }
    public DbSet<Company> Companies { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=users.db");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .OwnsOne(u => u.Address)
            .OwnsOne(a => a.Geo);
        modelBuilder.Entity<User>()
            .OwnsOne(u => u.Company);

        modelBuilder.Entity<User>()
            .Property(u => u.EmailAlias)
            .HasComputedColumnSql("SUBSTR([Name], 1, 1) || SUBSTR([Name], INSTR([Name], ' ') + 1) || '@ibsat.com'");
    }
}