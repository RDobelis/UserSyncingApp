using Microsoft.EntityFrameworkCore;
using UserSyncingApp.ServiceModel.Types;

namespace UserSyncingApp.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Geo> Geos { get; set; }
    public DbSet<Company> Companies { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .OwnsOne(u => u.Address)
            .OwnsOne(a => a.Geo);
        modelBuilder.Entity<User>()
            .OwnsOne(u => u.Company);

        modelBuilder.Entity<User>()
            .Property(u => u.EmailAlias)
            .HasComputedColumnSql(GetSqlQueryForEmailAlias());
    }

    private static string GetSqlQueryForEmailAlias() =>
        "SUBSTR([Name], 1, 1) || " +
        "TRIM(SUBSTR(SUBSTR([Name], INSTR([Name], ' ') + 1), 1, " +
        "CASE WHEN INSTR(SUBSTR([Name], INSTR([Name], ' ') + 1), ' ') > 0 THEN INSTR(SUBSTR([Name], INSTR([Name], ' ') + 1), ' ') ELSE LENGTH([Name]) END)) " +
        "|| '@ibsat.com'";
}