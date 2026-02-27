using backend.Data;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace backend.Services;

public class UserService : IUserRepository
{

    private readonly DataContext _dbContext;
    public UserService(DataContext context) => _dbContext = context;


    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByEmailAsync(string? email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> CreateUserAsync(User u)
    {
        await _dbContext.Users.AddAsync(u);
        await _dbContext.SaveChangesAsync();

        return u;
    }

    public async Task<User> UpdateUserAsync(User u)
    {
        u.UpdatedAtUTC = DateTime.UtcNow;

        _dbContext.Users.Update(u);
        await _dbContext.SaveChangesAsync();

        return u;
    }

    public async Task DeleteUserAsync(User u)
    {
        _dbContext.Users.Remove(u);
        await _dbContext.SaveChangesAsync();
    }
}
