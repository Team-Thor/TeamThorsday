using System.Net.Http.Json;
using Shared.DTOs;

namespace Team_Thorsday.Services;

public class SteamDashboardService(HttpClient http)
{
    public Task<List<SteamSpyGameDto>?> GetTopGamesAsync()
        => http.GetFromJsonAsync<List<SteamSpyGameDto>>("api/games/top");

    public Task<List<SteamSpyGameDto>?> GetNewReleasesAsync()
        => http.GetFromJsonAsync<List<SteamSpyGameDto>>("api/games/new");

    public Task<List<GenreStatsDto>?> GetGenreStatsAsync()
        => http.GetFromJsonAsync<List<GenreStatsDto>>("api/games/genres");
}
