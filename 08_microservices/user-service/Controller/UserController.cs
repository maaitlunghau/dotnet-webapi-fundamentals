using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using user_service.Data;
using user_service.Model;

namespace user_service.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dbContext;

        public UserController(DataContext dbContext)
            => _dbContext = dbContext;

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(User userLogin)
        {
            var user = _dbContext.Users.FirstOrDefault
                (u => u.Email == userLogin.Email
                    && u.Password == userLogin.Password);
            if (user == null)
                return Unauthorized();

            return Ok(user);
        }
    }
}
