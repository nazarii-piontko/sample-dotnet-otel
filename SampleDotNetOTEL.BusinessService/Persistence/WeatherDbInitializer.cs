using Microsoft.EntityFrameworkCore;
using Npgsql;
using Polly;

namespace SampleDotNetOTEL.BusinessService.Persistence;

public sealed class WeatherDbInitializer(WeatherDbContext dbContext)
{
    private static readonly string[] Summaries =
    {
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching"
    };

    public async Task InitAsync()
    {
        await Policy
            .Handle<NpgsqlException>()
            .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(1))
            .ExecuteAsync(async () =>
            {
                await dbContext.Database.MigrateAsync();

                if (!await dbContext.WeatherEntries.AnyAsync())
                {
                    dbContext.WeatherEntries
                        .AddRange(Enumerable.Range(1, 5)
                            .Select(index => new WeatherEntry(
                                DateTime.UtcNow.AddDays(index),
                                Random.Shared.Next(-20, 55),
                                Summaries[Random.Shared.Next(Summaries.Length)])));
                    await dbContext.SaveChangesAsync();
                }
            });
    }
}