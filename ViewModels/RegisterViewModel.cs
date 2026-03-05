using Team_Thorsday.Models;
using Team_Thorsday.Services;

namespace Team_Thorsday.ViewModels;

public class RegisterViewModel
{
    private readonly Supabase.Client _supabase;
    private readonly IDatabaseService _dbService;

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; private set; } = "";
    public bool IsLoading { get; private set; } = false;

    public RegisterViewModel(Supabase.Client supabase, IDatabaseService dbService)
    {
        _supabase = supabase;
        _dbService = dbService;
    }

    public async Task<bool> RegisterAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = "";

            await _supabase.Auth.SignUp(Email, Password);

            var newUser = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
            };

            await _dbService.AddUserAsync(newUser);
            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
        finally
        {
            IsLoading = false;
        }
    }
}
