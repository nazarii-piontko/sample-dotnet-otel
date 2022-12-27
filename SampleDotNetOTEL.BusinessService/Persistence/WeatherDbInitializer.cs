using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;

namespace SampleDotNetOTEL.BusinessService.Persistence;

public sealed class WeatherDbInitializer
{
    private readonly WeatherDbContext _dbContext;

    public WeatherDbInitializer(WeatherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InitAsync()
    {
        await Policy
            .Handle<NpgsqlException>()
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1))
            .ExecuteAsync(async () =>
            {
                await _dbContext.Database.MigrateAsync();

                if (!await _dbContext.WeatherEntries.AnyAsync())
                {
                    var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };
                    _dbContext.WeatherEntries.AddRange(Enumerable.Range(1, 5).Select(index => new WeatherEntry
                    {
                        Date = DateTime.UtcNow.AddDays(index),
                        TemperatureC = Random.Shared.Next(-20, 55),
                        Summary = summaries[Random.Shared.Next(summaries.Length)]
                    }));
                    await _dbContext.SaveChangesAsync();
                }
            });
    }
}