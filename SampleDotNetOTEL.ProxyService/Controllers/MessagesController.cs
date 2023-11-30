using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController(MessageBroker messageBroker) : ControllerBase
{
    [HttpPost]
    public IActionResult Post([FromBody] MessageRequest request)
    {
        messageBroker.PublishMessage(request.Message);
        return Ok();
    }

    public sealed class MessageRequest
    {
        [Required]
        public string Message { get; set; }
    }
}