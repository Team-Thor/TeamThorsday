using System.Text.Json;
using Shared.DTOs;

namespace Backend.Services;

public class SteamService(HttpClient http)
{
    // live ccu, no cache since it changes constantly
    public async Task<int> GetCurrentPlayersAsync(int appId)
    {
        var json = await http.GetStringAsync(
            $"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={appId}");

        // couldn't get this to deserialize into a class, this works though
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("response")
            .GetProperty("player_count")
            .GetInt32();
    }

    public async Task<SteamReviewsDto?> GetReviewsAsync(int appId)
    {
        // needs json=1 or steam sends back html for some reason
        var json = await http.GetStringAsync(
            $"https://store.steampowered.com/appreviews/{appId}?json=1&num_per_page=20&filter=recent");

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("query_summary", out var summary))
            return null;

        var result = new SteamReviewsDto
        {
            ReviewScoreDesc = summary.GetProperty("review_score_desc").GetString() ?? "",
            TotalPositive = summary.GetProperty("total_positive").GetInt32(),
            TotalNegative = summary.GetProperty("total_negative").GetInt32(),
            TotalReviews = summary.GetProperty("total_reviews").GetInt32()
        };

        if (root.TryGetProperty("reviews", out var reviewsArr))
        {
            foreach (var r in reviewsArr.EnumerateArray())
            {
                result.Reviews.Add(new SteamReviewDto
                {
                    VotedUp = r.GetProperty("voted_up").GetBoolean(),
                    PlaytimeAtReview = r.TryGetProperty("author", out var author)
                        ? author.GetProperty("playtime_at_review").GetInt32()
                        : 0,
                    Review = r.GetProperty("review").GetString() ?? ""
                });
            }
        }

        return result;
    }

    public async Task<List<SteamSpyGameDto>> GetNewReleasesAsync()
    {
        try
        {
            var json = await http.GetStringAsync("https://store.steampowered.com/api/featuredcategories/");
            using var doc = JsonDocument.Parse(json);

            var items = doc.RootElement.GetProperty("new_releases").GetProperty("items");
            var result = new List<SteamSpyGameDto>();

            foreach (var item in items.EnumerateArray())
            {
                result.Add(new SteamSpyGameDto
                {
                    AppId = item.GetProperty("id").GetInt32(),
                    Name = item.GetProperty("name").GetString() ?? "",
                    Price = item.TryGetProperty("final_price", out var price)
                        ? price.GetInt32().ToString()
                        : "0"
                });
            }

            return result;
        }
        catch
        {
            // steam changes this format sometimes
            return new List<SteamSpyGameDto>();
        }
    }
}
