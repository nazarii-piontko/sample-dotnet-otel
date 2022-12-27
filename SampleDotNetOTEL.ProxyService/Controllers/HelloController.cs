using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly BusinessServiceClient _client;

    public HelloController(BusinessServiceClient client)
    {
        _client = client;
    }

    [HttpGet]
    public async Task<IActionResult> Get(string? name)
    {
        var response = string.IsNullOrEmpty(name)
            ? await _client.GetHelloAsync()
            : await _client.GetHelloAsync(name);
        return Ok(response);
    }
}