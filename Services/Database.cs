using Team_Thorsday.Models;

namespace Team_Thorsday.Services;

public class DatabaseService : IDatabaseService
{
    private readonly Supabase.Client _supabaseClient;

    public DatabaseService(Supabase.Client supabaseClient) => _supabaseClient = supabaseClient;

    public async Task<List<User>> GetUsersAsync()
    {
        var response = await _supabaseClient.From<User>().Get();
        return response.Models;
    }

    public async Task<User> AddUserAsync(User newUser)
    {
        var response = await _supabaseClient.From<User>().Insert(newUser);
        return response.Models.First();
    }

    public async Task<ApiKey> CreateApiKeyAsync(int userId)
    {
        var newKey = new ApiKey
        {
            UserId = userId,
            Key = "sk-" + Guid.NewGuid().ToString("N").Substring(0, 20),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var response = await _supabaseClient.From<ApiKey>().Insert(newKey);
        return response.Models.First();
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        var response = await _supabaseClient.From<User>()
            .Where(u => u.Id == id)
            .Get();

        return response.Models.FirstOrDefault();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var response = await _supabaseClient.From<User>()
            .Where(u => u.Email == email)
            .Get();

        return response.Models.FirstOrDefault();
    }
}