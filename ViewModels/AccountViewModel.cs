using Team_Thorsday.Models;
using Team_Thorsday.Services;

namespace Team_Thorsday.ViewModels;

public class AccountViewModel
{
    private readonly IDatabaseService _dbService;
    private readonly Supabase.Client _supabase;

    public User? User { get; private set; }
    public bool IsLoading { get; private set; } = true;
    public string ErrorMessage { get; private set; } = "";

    public AccountViewModel(IDatabaseService dbService, Supabase.Client supabase)
    {
        _dbService = dbService;
        _supabase = supabase;
    }

    public async Task LoadUserAsync()
    {
        try
        {
            IsLoading = true;
            var session = _supabase.Auth.CurrentSession;

            if (session?.User?.Email is null)
            {
                ErrorMessage = "Not logged in.";
                return;
            }

            User = await _dbService.GetUserByEmailAsync(session.User.Email);
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
}