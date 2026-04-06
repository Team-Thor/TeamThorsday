using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Backend.Controllers;

/// <summary>
/// Main API controller for game-related data.
/// Combines data from SteamSpy, CheapShark, and the custom Steam service.
/// Route prefix: /api/games
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController(
    SteamSpyService steamSpy,      // Service for SteamSpy API calls
    CheapSharkService cheapShark,  // Service for current game deals
    SteamService steam)            // Custom Steam service (new releases, current players, reviews)
    : ControllerBase
{
    /// <summary>
    /// Hardcoded list of genres to analyze. 
    /// There is no single SteamSpy endpoint that returns all genres, so we define them here.
    /// </summary>
    private static readonly string[] Genres = new string[]
    {
        "Action", "Adventure", "Casual", "Early Access", "Free to Play",
        "Indie", "Massively Multiplayer", "RPG", "Racing", "Simulation",
        "Sports", "Strategy", "Anime"
    };

    /// <summary>
    /// GET /api/games/genres
    /// Returns aggregated statistics for each genre (game count, avg review score, 
    /// avg price, avg playtime, and total CCU).
    /// </summary>
    [HttpGet("genres")]
    public async Task<IActionResult> GetGenreStats()
    {
        var results = new List<GenreStatsDto>();

        foreach (var genre in Genres)
        {
            // Fetch games belonging to this genre from SteamSpy
            var games = await steamSpy.GetGamesByGenreAsync(genre);
            if (games.Count == 0) continue;

            // Calculate average review score (positive review percentage)
            double avgScore = 0;
            int scored = 0;
            foreach (var g in games)
            {
                int total = g.Positive + g.Negative;
                if (total > 0)
                {
                    avgScore += (double)g.Positive / total * 100;
                    scored++;
                }
            }

            // Calculate average price (skipping free games to avoid skewing the average)
            double avgPrice = 0;
            int pricedCount = 0;
            foreach (var g in games)
            {
                if (g.Price != "0" && g.Price != "" && int.TryParse(g.Price, out var cents))
                {
                    avgPrice += cents;
                    pricedCount++;
                }
            }
            if (pricedCount > 0)
                avgPrice = avgPrice / pricedCount / 100.0;

            // Calculate average playtime across all games in the genre
            int totalPlaytime = games.Sum(g => g.AveragePlaytimeMinutes);

            results.Add(new GenreStatsDto
            {
                Genre = genre,
                GameCount = games.Count,
                AvgReviewScore = scored > 0 ? Math.Round(avgScore / scored, 1) : 0,
                AvgPriceUsd = Math.Round(avgPrice, 2),
                AvgPlaytimeMinutes = totalPlaytime / games.Count,
                TotalCcu = games.Sum(g => g.PeakConcurrentUsersYesterday)
            });
        }

        return Ok(results);
    }

    /// <summary>
    /// GET /api/games/top
    /// Returns the top 100 most played/owned games from SteamSpy.
    /// </summary>
    [HttpGet("top")]
    public async Task<IActionResult> GetTopGames()
    {
        var games = await steamSpy.GetTop100Async();
        return Ok(games);
    }

    /// <summary>
    /// GET /api/games/new
    /// Returns recently released games from the Steam service.
    /// </summary>
    [HttpGet("new")]
    public async Task<IActionResult> GetNewReleases()
    {
        var games = await steam.GetNewReleasesAsync();
        return Ok(games);
    }

    /// <summary>
    /// GET /api/games/tag/{tag}
    /// Returns games matching a specific SteamSpy tag (e.g., "Roguelite", "FPS").
    /// </summary>
    [HttpGet("tag/{tag}")]
    public async Task<IActionResult> GetByTag(string tag)
    {
        var games = await steamSpy.GetGamesByTagAsync(tag);
        return Ok(games);
    }

    /// <summary>
    /// GET /api/games/{appId}
    /// Returns combined data for a single game:
    ///   - Detailed info from SteamSpy
    ///   - Current deals from CheapShark
    /// </summary>
    [HttpGet("{appId}")]
    public async Task<IActionResult> GetGame(int appId)
    {
        var steamData = await steamSpy.GetAppDetailsAsync(appId);
        if (steamData == null) 
            return NotFound();

        var deals = await cheapShark.GetDealsByAppIdAsync(appId.ToString());

        var result = new GameDetailDto
        {
            AppId = steamData.AppId,
            Name = steamData.Name,
            Developer = steamData.Developer,
            Publisher = steamData.Publisher,
            Owners = steamData.Owners,
            Genre = steamData.Genre,
            Positive = steamData.Positive,
            Negative = steamData.Negative,
            AveragePlaytimeMinutes = steamData.AveragePlaytimeMinutes,
            Tags = steamData.Tags,
            Deals = deals
        };

        return Ok(result);
    }

    /// <summary>
    /// GET /api/games/{appId}/ccu
    /// Returns the current number of players online for a specific game.
    /// </summary>
    [HttpGet("{appId}/ccu")]
    public async Task<IActionResult> GetCcu(int appId)
    {
        var count = await steam.GetCurrentPlayersAsync(appId);
        return Ok(new { appId, playerCount = count });
    }

    /// <summary>
    /// GET /api/games/{appId}/reviews
    /// Returns review data for a specific game from the Steam service.
    /// </summary>
    [HttpGet("{appId}/reviews")]
    public async Task<IActionResult> GetReviews(int appId)
    {
        var reviews = await steam.GetReviewsAsync(appId);
        if (reviews == null) 
            return NotFound();

        return Ok(reviews);
    }
}