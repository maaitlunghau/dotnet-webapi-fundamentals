using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Models;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public AccountController(DataContext dbContext)
            => _dbContext = dbContext;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accounts = await _dbContext.Accounts.ToListAsync();
            return Ok(accounts);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetSingle(Guid id)
        {
            var account = await _dbContext.Accounts.FindAsync(id);
            if (account is null) return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Account acc)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newAcc = await _dbContext.Accounts.AddAsync(acc);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSingle),
                new { id = acc.Id },
                acc
            );
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update([FromBody] Account acc)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedAcc = _dbContext.Update(acc);
            await _dbContext.SaveChangesAsync();

            return Ok(updatedAcc);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existingAcc = await _dbContext.Accounts.FindAsync(id);
            if (existingAcc is null)
                return NotFound($"Account with {id} not found");

            _dbContext.Accounts.Remove(existingAcc);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
