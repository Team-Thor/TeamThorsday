using Team_Thorsday.Services;

namespace Team_Thorsday.ViewModels;

public class LoginViewModel
{
    private readonly Supabase.Client _supabase;

    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ErrorMessage { get; private set; } = "";
    public bool IsLoading { get; private set; } = false;

    public LoginViewModel(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<bool> LoginAsync()
    {
        try
        {
            IsLoading = true;
            ErrorMessage = "";
            await _supabase.Auth.SignIn(Email, Password);
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