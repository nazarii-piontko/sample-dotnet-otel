using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController(BusinessServiceClient client) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await client.GetWeatherAsync());
    }
}