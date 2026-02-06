using Shared.Domain;

namespace backend.Repository;

public interface IUserRepository
{
    public Task<IEnumerable<User>> GetAllUsersAsync();
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByIdAsync(Guid? id);
    public Task<User> AddUserAsync(User user);
    public Task<User> UpdateUserAsync(User user);
    public Task DeleteUserAsync(User user);
}