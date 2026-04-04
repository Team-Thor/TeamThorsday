using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Team_Thorsday;
using Team_Thorsday.Services;
using Team_Thorsday.ViewModels;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<UserDirectoryViewModel>();
builder.Services.AddScoped<LoginViewModel>();
builder.Services.AddScoped<AccountViewModel>();
builder.Services.AddScoped<RegisterViewModel>();

var supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url is not configured");
var supabaseKey = builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key is not configured");

var options = new Supabase.SupabaseOptions()
{
    AutoRefreshToken = true,
    AutoConnectRealtime = false,
};

var supabase = new Supabase.Client(supabaseUrl, supabaseKey, options);
await supabase.InitializeAsync();

builder.Services.AddSingleton(provider => supabase);
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(_ => new SteamDashboardService(
    new HttpClient { BaseAddress = new Uri("http://localhost:5003") }));

await builder.Build().RunAsync();