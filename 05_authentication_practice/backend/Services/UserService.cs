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


    public async Task<User> AddUserAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }


    public async Task<User> UpdateUserAsync(User u)
    {
        var existingUser = await GetUserByIdAsync(u.Id);
        if (existingUser is null)
            throw new KeyNotFoundException($"User with ID {u.Id} not found.");

        existingUser.Email = u.Email;
        existingUser.Role = u.Role;

        await _dbContext.SaveChangesAsync();
        return existingUser;
    }


    public async Task DeleteUserAsync(User u)
    {
        _dbContext.Users.Remove(u);
        await _dbContext.SaveChangesAsync();
    }
}