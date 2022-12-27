using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SampleDotNetOTEL.BusinessService.Controllers;

[ApiController]
[Route("[controller]")]
public class HelloController : ControllerBase
{
    private readonly ErrorResponsePolicy _errorResponsePolicy;

    public HelloController(ErrorResponsePolicy errorResponsePolicy)
    {
        _errorResponsePolicy = errorResponsePolicy;
    }

    [HttpGet]
    public IActionResult Get()
    {
        if (_errorResponsePolicy.IsProduceError())
            return StatusCode(500);
        return Ok("Hello World");
    }
    
    [HttpPost]
    public IActionResult Post([FromBody] HelloRequest request)
    {
        if (_errorResponsePolicy.IsProduceError())
            return StatusCode(500);
        return Ok($"Hello {request.Name}");
    }
    
    public sealed class HelloRequest
    {
        [Required]
        [MinLength(2)]
        public string? Name { get; set; }
    }
}