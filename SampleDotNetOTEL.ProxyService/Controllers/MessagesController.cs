using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SampleDotNetOTEL.ProxyService.ExternalServices;

namespace SampleDotNetOTEL.ProxyService.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase
{
    private readonly MessageBroker _messageBroker;

    public MessagesController(MessageBroker messageBroker)
    {
        _messageBroker = messageBroker;
    }

    [HttpPost]
    public IActionResult Post([FromBody] MessageRequest request)
    {
        _messageBroker.PublishMessage(request.Message);
        return Ok();
    }

    public sealed class MessageRequest
    {
        [Required]
        public string Message { get; set; }
    }
}