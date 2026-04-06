using System.Text.Json;
using Backend.Models;
using Shared.DTOs;

namespace Backend.Services;

public class SteamSpyService(HttpClient http)
{
    private const string BaseUrl = "https://steamspy.com/api.php";

    // just cache stuff in memory so we don't spam the api
    private static readonly Dictionary<string, object> Cache = new();
    private static readonly Dictionary<string, DateTime> CacheTime = new();

    public async Task<List<SteamSpyGameDto>> GetGamesByTagAsync(string tag)
    {
        var key = $"tag_{tag}";

        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (List<SteamSpyGameDto>)Cache[key];

        var json = await http.GetStringAsync($"{BaseUrl}?request=tag&tag={Uri.EscapeDataString(tag)}");
        var raw = JsonSerializer.Deserialize<Dictionary<string, SteamSpyBasicApp>>(json);

        if (raw == null) return new List<SteamSpyGameDto>();

        var result = raw.Values.Select(app => new SteamSpyGameDto
        {
            AppId = app.AppId,
            Name = app.Name,
            Developer = app.Developer,
            Publisher = app.Publisher,
            Owners = app.Owners,
            Positive = app.Positive,
            Negative = app.Negative,
            AveragePlaytimeMinutes = app.AverageForever,
            PeakConcurrentUsersYesterday = app.PeakConcurrentUsersYesterday,
            Price = app.Price
        }).ToList();

        Cache[key] = result;
        CacheTime[key] = DateTime.Now;
        return result;
    }

    // basically the same as tag but for genre
    public async Task<List<SteamSpyGameDto>> GetGamesByGenreAsync(string genre)
    {
        var key = $"genre_{genre}";

        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (List<SteamSpyGameDto>)Cache[key];

        var json = await http.GetStringAsync($"{BaseUrl}?request=genre&genre={Uri.EscapeDataString(genre)}");
        var raw = JsonSerializer.Deserialize<Dictionary<string, SteamSpyBasicApp>>(json);

        if (raw == null) return new List<SteamSpyGameDto>();

        var result = raw.Values.Select(app => new SteamSpyGameDto
        {
            AppId = app.AppId,
            Name = app.Name,
            Developer = app.Developer,
            Publisher = app.Publisher,
            Owners = app.Owners,
            Positive = app.Positive,
            Negative = app.Negative,
            AveragePlaytimeMinutes = app.AverageForever,
            PeakConcurrentUsersYesterday = app.PeakConcurrentUsersYesterday,
            Price = app.Price
        }).ToList();

        Cache[key] = result;
        CacheTime[key] = DateTime.Now;
        return result;
    }

    public async Task<List<SteamSpyGameDto>> GetTop100Async()
    {
        var key = "top100";

        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (List<SteamSpyGameDto>)Cache[key];

        var json = await http.GetStringAsync($"{BaseUrl}?request=top100in2weeks");
        var raw = JsonSerializer.Deserialize<Dictionary<string, SteamSpyBasicApp>>(json);

        if (raw == null) return new List<SteamSpyGameDto>();

        var result = raw.Values.Select(app => new SteamSpyGameDto
        {
            AppId = app.AppId,
            Name = app.Name,
            Developer = app.Developer,
            Publisher = app.Publisher,
            Owners = app.Owners,
            Positive = app.Positive,
            Negative = app.Negative,
            AveragePlaytimeMinutes = app.AverageForever,
            PeakConcurrentUsersYesterday = app.PeakConcurrentUsersYesterday,
            Price = app.Price
        }).ToList();

        Cache[key] = result;
        CacheTime[key] = DateTime.Now;
        return result;
    }

    public async Task<SteamSpyGameDto?> GetAppDetailsAsync(int appId)
    {
        var key = $"app_{appId}";

        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (SteamSpyGameDto)Cache[key];

        var json = await http.GetStringAsync($"{BaseUrl}?request=appdetails&appid={appId}");
        var raw = JsonSerializer.Deserialize<SteamSpyAppDetails>(json);

        if (raw == null) return null;

        var result = new SteamSpyGameDto
        {
            AppId = raw.AppId,
            Name = raw.Name,
            Developer = raw.Developer,
            Publisher = raw.Publisher,
            Owners = raw.Owners,
            Genre = raw.Genre,
            Positive = raw.Positive,
            Negative = raw.Negative,
            AveragePlaytimeMinutes = raw.AverageForever,
            PeakConcurrentUsersYesterday = raw.PeakConcurrentUsersYesterday,
            Price = raw.Price,
            Tags = raw.Tags ?? new Dictionary<string, int>()
        };

        Cache[key] = result;
        CacheTime[key] = DateTime.Now;
        return result;
    }
}