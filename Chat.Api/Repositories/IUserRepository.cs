using Chat.Api.Entities;

namespace Chat.Api.Repositories;

public interface IUserRepository
{
    Task<List<User>> GetAllUsers();
    Task<User> GetUserById(Guid id);
    Task<User?> GetUserByUsername(string username);

    Task AddUser(User user);
    Task UpdateUser(User user);
    Task DeleteUser(User user);
}