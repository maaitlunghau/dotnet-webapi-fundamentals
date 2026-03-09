using LoyalService.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoyalService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoyaltyController : ControllerBase
    {
        private readonly LoyaltyCustomerService _loyaltyService;
        public LoyaltyController(LoyaltyCustomerService loyaltyService)
        {
            _loyaltyService = loyaltyService;
        }

        // GET: api/loyalty/customers
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _loyaltyService.GetAllCustomers();
            return Ok(customers);
        }

        // GET: api/loyalty/customer/{userId}
        [HttpGet("customer/{userId}")]
        public async Task<IActionResult> GetCustomer(int userId)
        {
            var customer = await _loyaltyService.GetCustomer(userId);

            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer);
        }
        // POST: api/loyalty/earn
        [HttpPost("earn")]
        public async Task<IActionResult> EarnPoints(
            int userId,
            int points,
            string description)
        {
            await _loyaltyService.AddPoints(userId, points, description);
            return Ok("Points added successfully");
        }

        // POST: api/loyalty/redeem
        [HttpPost("redeem")]
        public async Task<IActionResult> RedeemPoints(
            int userId,
            int points,
            string description)
        {
            await _loyaltyService.RedeemPoints(userId, points, description);
            return Ok("Points redeemed successfully");
        }

        // GET: api/loyalty/history/{userId}
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetHistory(int userId)
        {
            var history = await _loyaltyService.GetPointHistory(userId);
            return Ok(history);
        }
    }
}
