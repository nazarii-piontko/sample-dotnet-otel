using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController(BusinessServiceClient client) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(string? name)
    {
        var response = string.IsNullOrEmpty(name)
            ? await client.GetHelloAsync()
            : await client.GetHelloAsync(name);
        return Ok(response);
    }
}