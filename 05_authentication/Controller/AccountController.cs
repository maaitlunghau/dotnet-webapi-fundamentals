using _05_authentication.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _05_authentication.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dbContext;
        public AccountController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accounts = await _dbContext.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Account account)
        {
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            return Created("", account);
        }
    }
}
