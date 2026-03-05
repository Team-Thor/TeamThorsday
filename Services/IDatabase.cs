using Team_Thorsday.Models;

namespace Team_Thorsday.Services;

public interface IDatabaseService
{
    Task<List<User>> GetUsersAsync();
    Task<User> AddUserAsync(User user);
    Task<ApiKey> CreateApiKeyAsync(int userId);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
}

