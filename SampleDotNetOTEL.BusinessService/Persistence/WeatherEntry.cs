namespace SampleDotNetOTEL.BusinessService.Persistence;

public class WeatherEntry
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }
}