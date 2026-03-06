using direct.Models;
using direct.Services;
using Microsoft.AspNetCore.Mvc;

namespace direct.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private IRabbitMQPublisher _rabbitMqPublisher;

        public PublishController(IRabbitMQPublisher rabbitMqPublisher)
            => _rabbitMqPublisher = rabbitMqPublisher;

        [HttpPost]
        public async Task<IActionResult> Publish(OrderCreate orderCreate)
        {
            await _rabbitMqPublisher.PublishAsync("order.created", orderCreate);
            return Ok(
                $"Published {orderCreate.OrderId} with {orderCreate.Price} at {DateTime.Now}"
            );
        }
    }
}
