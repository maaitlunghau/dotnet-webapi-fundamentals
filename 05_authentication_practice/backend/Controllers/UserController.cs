using System.Net;
using backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user is null) return NotFound($"User with ID {id} not found.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User u)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int costFactor = 12;

            try
            {
                // hashing password
                u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password, costFactor);

                var userCreated = await _userRepository.AddUserAsync(u);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = userCreated.Id },
                    userCreated
                );
            }
            catch (Exception ex)
            {
                var details = $"Exception message: {ex.Message} Details: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, details);
            }
        }


        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User u)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                u.Id = id;

                var userUpdated = await _userRepository.UpdateUserAsync(u);
                return Ok(userUpdated);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser is null) return NotFound($"User with ID {id} not found.");

                await _userRepository.DeleteUserAsync(existingUser);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
