using System.Text.Json;
using Shared.DTOs;

namespace Backend.Services;

/// <summary>
/// Service for interacting with official Steam Web APIs and the Steam Store.
/// Handles current player count, recent reviews, and new releases.
/// </summary>
public class SteamService(HttpClient http)
{
    /// <summary>
    /// Gets the current number of players online for a specific Steam app.
    /// No caching is used because player counts change frequently.
    /// </summary>
    public async Task<int> GetCurrentPlayersAsync(int appId)
    {
        var json = await http.GetStringAsync(
            $"https://api.steampowered.com/ISteamUserStats/GetNumberOfCurrentPlayers/v1/?appid={appId}");

        // Manual JSON parsing because the response structure is simple but nested.
        // Deserializing into a strongly-typed class was unreliable.
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
            .GetProperty("response")
            .GetProperty("player_count")
            .GetInt32();
    }

    /// <summary>
    /// Fetches recent reviews for a game from the Steam Store.
    /// Returns a summary (score description, totals) plus up to 20 recent reviews.
    /// </summary>
    public async Task<SteamReviewsDto?> GetReviewsAsync(int appId)
    {
        // The 'json=1' parameter is required — without it Steam sometimes returns HTML instead of JSON.
        var json = await http.GetStringAsync(
            $"https://store.steampowered.com/appreviews/{appId}?json=1&num_per_page=20&filter=recent");

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // If there's no query_summary, the request likely failed or returned no data
        if (!root.TryGetProperty("query_summary", out var summary))
            return null;

        var result = new SteamReviewsDto
        {
            ReviewScoreDesc = summary.GetProperty("review_score_desc").GetString() ?? "",
            TotalPositive = summary.GetProperty("total_positive").GetInt32(),
            TotalNegative = summary.GetProperty("total_negative").GetInt32(),
            TotalReviews = summary.GetProperty("total_reviews").GetInt32()
        };

        // Parse individual recent reviews if present
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

    /// <summary>
    /// Fetches newly released or featured games from the Steam Store "New Releases" section.
    /// Returns a lightweight list of SteamSpyGameDto objects.
    /// </summary>
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
            // Steam occasionally changes the structure of this endpoint.
            // Return empty list gracefully instead of crashing.
            return new List<SteamSpyGameDto>();
        }
    }
}