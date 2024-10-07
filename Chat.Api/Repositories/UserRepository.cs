using Chat.Api.Context;
using Chat.Api.Entities;
using Chat.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Chat.Api.Repositories;

public class UserRepository(ChatDbContext context) : IUserRepository
{
    private readonly ChatDbContext _context = context;

    public async Task<List<User>> GetAllUsers()
    {
        var users = await _context.Users.AsNoTracking().ToListAsync();
        return users;
    }

    public async Task<User> GetUserById(Guid id)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        if (user is null)
            throw new UserNotFoundException();

        return user;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u =>
            u.Username.ToLower() == username.ToLower());
        
        //Example from database => example
        //exaMple from user => example
        
        return user;
    }

    public async Task AddUser(User user)
    {
       _context.Users.Add(user);
       await _context.SaveChangesAsync();
    }

    public async Task UpdateUser(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUser(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}