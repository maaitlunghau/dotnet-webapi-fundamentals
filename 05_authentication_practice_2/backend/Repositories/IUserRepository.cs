using Shared.Domain;

namespace backend.Repositories;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetAllUsersAsync();
    public Task<User?> GetUserByIdAsync(Guid id);
    public Task<User?> GetUserByEmailAsync(string? email);
    public Task<User> CreateUserAsync(User u);
    public Task<User> UpdateUserAsync(User u);
    public Task DeleteUserAsync(User u);
}
