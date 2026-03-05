using Team_Thorsday.Models;
using Team_Thorsday.Services;
using Newtonsoft.Json;

namespace Team_Thorsday.ViewModels;

public class UserDirectoryViewModel
{
    private readonly IDatabaseService _dbService;

    public List<User> Users { get; private set; } = new();
    public User NewUser { get; set; } = new();
    public string ErrorMessage { get; private set; } = "";
    public string RawJson { get; private set; } = "";
    public bool IsLoading { get; private set; } = true;
    public string ActiveTab { get; set; } = "table";

    public UserDirectoryViewModel(IDatabaseService dbService)
    {
        _dbService = dbService;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            Users = await _dbService.GetUsersAsync();
            RawJson = JsonConvert.SerializeObject(Users, Formatting.Indented);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task AddUserAsync()
    {
        try
        {
            await _dbService.AddUserAsync(NewUser);
            NewUser = new User();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task GenerateKeyAsync(int userId)
    {
        await _dbService.CreateApiKeyAsync(userId);
    }
}