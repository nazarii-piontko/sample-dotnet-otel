using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleDotNetOTEL.BusinessService.Persistence;

namespace SampleDotNetOTEL.BusinessService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController : ControllerBase
{
    private readonly WeatherDbContext _weatherDbContext;
    private readonly ErrorResponsePolicy _errorResponsePolicy;

    public WeatherController(WeatherDbContext weatherDbContext, ErrorResponsePolicy errorResponsePolicy)
    {
        _weatherDbContext = weatherDbContext;
        _errorResponsePolicy = errorResponsePolicy;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (_errorResponsePolicy.IsProduceError())
            return StatusCode(500);
        return Ok(await _weatherDbContext.WeatherEntries.ToListAsync());
    }
}