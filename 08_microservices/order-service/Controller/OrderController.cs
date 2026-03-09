using Microsoft.AspNetCore.Mvc;
using order_service.Data;
using order_service.Models;

namespace order_service.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public OrderController(DataContext context)
            => _dbContext = context;

        [HttpPost]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return Ok(order);
        }
    }
}
