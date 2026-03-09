using Microsoft.AspNetCore.Mvc;
using user_service.Data;
using user_service.DTOs;
using user_service.Model;
using user_service.Services;

namespace user_service.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IRabbitMqPublisher _rabbitMqPublisher;

        public UserController(DataContext dbContext, IRabbitMqPublisher rabbitMqPublisher)
        {
            _dbContext = dbContext;
            _rabbitMqPublisher = rabbitMqPublisher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var messageDto = new MessageDTO
            {
                UserId = user.Id,
                Message = user.Email
            };

            await _rabbitMqPublisher.PublishAsync("user_points", messageDto);

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
