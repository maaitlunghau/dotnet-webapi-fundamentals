using System.Net;
using backend.DTOs;
using backend.Repositories;
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
            => _userRepository = userRepository;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();

                var response = users.Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    CreatedAt = u.CreatedAtUTC,
                    UpdatedAt = u.UpdatedAtUTC
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = $"User with {id} not found." });

                var response = new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    CreatedAt = user.CreatedAtUTC,
                    UpdatedAt = user.UpdatedAtUTC
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
                if (existingUser != null)
                    return Conflict(new { message = "Email already exists." });

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = "user"
                };

                var newlyCreatedUser = await _userRepository.CreateUserAsync(user);

                var response = new UserResponseDto
                {
                    Id = newlyCreatedUser.Id,
                    Name = newlyCreatedUser.Name,
                    Email = newlyCreatedUser.Email,
                    Role = newlyCreatedUser.Role,
                    CreatedAt = newlyCreatedUser.CreatedAtUTC,
                    UpdatedAt = newlyCreatedUser.UpdatedAtUTC
                };

                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser == null)
                    return NotFound(new { message = $"User with {id} not found." });

                if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != existingUser.Email)
                {
                    var emailExists = await _userRepository.GetUserByEmailAsync(dto.Email);
                    if (emailExists != null)
                        return Conflict(new { message = "Email already exists." });
                }

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    existingUser.Name = dto.Name;

                if (!string.IsNullOrWhiteSpace(dto.Email))
                    existingUser.Email = dto.Email;

                var updatedUser = await _userRepository.UpdateUserAsync(existingUser);

                var response = new UserResponseDto
                {
                    Id = updatedUser.Id,
                    Name = updatedUser.Name,
                    Email = updatedUser.Email,
                    Role = updatedUser.Role,
                    CreatedAt = updatedUser.CreatedAtUTC,
                    UpdatedAt = updatedUser.UpdatedAtUTC
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var existingUser = await _userRepository.GetUserByIdAsync(id);
                if (existingUser is null)
                    return NotFound(new { message = $"User with {id} not found." });

                await _userRepository.DeleteUserAsync(existingUser);

                return NoContent();
            }
            catch (Exception ex)
            {
                var detailMessage = $"Exception message: {ex.Message}. Detail: {ex}";
                return StatusCode((int)HttpStatusCode.InternalServerError, detailMessage);
            }
        }
    }
}
