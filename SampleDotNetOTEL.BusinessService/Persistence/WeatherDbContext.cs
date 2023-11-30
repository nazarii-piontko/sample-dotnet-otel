using Microsoft.EntityFrameworkCore;

namespace SampleDotNetOTEL.BusinessService.Persistence;

public class WeatherDbContext(DbContextOptions<WeatherDbContext> options)
    : DbContext(options)
{
    public DbSet<WeatherEntry> WeatherEntries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<WeatherEntry>()
            .HasKey(t => t.Date);
    }
}