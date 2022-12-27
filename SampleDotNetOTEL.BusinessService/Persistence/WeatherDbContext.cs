using Microsoft.EntityFrameworkCore;

namespace SampleDotNetOTEL.BusinessService.Persistence;

public class WeatherDbContext : DbContext
{
    public DbSet<WeatherEntry> WeatherEntries { get; set; } = null!;
    
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<WeatherEntry>()
            .HasKey(t => t.Date);
    }
}