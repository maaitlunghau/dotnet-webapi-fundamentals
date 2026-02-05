using backend.Data;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;

namespace backend.Services;

public class UserService : IUserRepository
{
    private readonly DataContext _dbContext;
    public UserService(DataContext dbContext) => _dbContext = dbContext;


    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _dbContext.Users.ToListAsync();
    }


    public async Task<User?> GetUserByIdAsync(Guid? id)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }


    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }


    public async Task AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }


    public async Task UpdateUserAsync(User user)
    {
        var existingUser = await GetUserByIdAsync(user.Id)
            ?? throw new KeyNotFoundException($"User with ID {user.Id} not found.");

        existingUser.Email = user.Email;
        existingUser.Role = user.Role;

        await _dbContext.SaveChangesAsync();
    }


    public async Task DeleteUserAsync(Guid? id)
    {
        var existingUser = await GetUserByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with ID {id} not found.");

        _dbContext.Users.Remove(existingUser);
        await _dbContext.SaveChangesAsync();
    }
}