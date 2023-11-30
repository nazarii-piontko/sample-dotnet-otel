using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleDotNetOTEL.BusinessService.Persistence;

namespace SampleDotNetOTEL.BusinessService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherController(WeatherDbContext weatherDbContext, ErrorResponsePolicy errorResponsePolicy) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (errorResponsePolicy.IsProduceError())
            return StatusCode(500);
        return Ok(await weatherDbContext.WeatherEntries.ToListAsync());
    }
}