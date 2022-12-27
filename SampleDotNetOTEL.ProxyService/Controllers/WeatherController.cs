using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly BusinessServiceClient _client;

    public WeatherController(BusinessServiceClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _client.GetWeatherAsync());
    }
}