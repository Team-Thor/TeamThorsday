using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController(SteamSpyService steamSpy, CheapSharkService cheapShark, SteamService steam) : ControllerBase
{
    // hardcoded since there's no api endpoint for this
    private static readonly string[] Genres = new string[]
    {
        "Action", "Adventure", "Casual", "Early Access", "Free to Play",
        "Indie", "Massively Multiplayer", "RPG", "Racing", "Simulation",
        "Sports", "Strategy", "Anime"
    };

    // GET /api/games/genres
    [HttpGet("genres")]
    public async Task<IActionResult> GetGenreStats()
    {
        var results = new List<GenreStatsDto>();

        foreach (var genre in Genres)
        {
            var games = await steamSpy.GetGamesByGenreAsync(genre);
            if (games.Count == 0) continue;

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

            // skip free games, messes up the average
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

            int totalPlaytime = 0;
            foreach (var g in games)
                totalPlaytime += g.AveragePlaytimeMinutes;

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

    [HttpGet("top")]
    public async Task<IActionResult> GetTopGames()
    {
        var games = await steamSpy.GetTop100Async();
        return Ok(games);
    }

    [HttpGet("new")]
    public async Task<IActionResult> GetNewReleases()
    {
        var games = await steam.GetNewReleasesAsync();
        return Ok(games);
    }

    // GET /api/games/tag/Roguelite
    [HttpGet("tag/{tag}")]
    public async Task<IActionResult> GetByTag(string tag)
    {
        var games = await steamSpy.GetGamesByTagAsync(tag);
        return Ok(games);
    }

    // GET /api/games/123456 - combined steamspy + cheapshark for one game
    [HttpGet("{appId}")]
    public async Task<IActionResult> GetGame(int appId)
    {
        var steamData = await steamSpy.GetAppDetailsAsync(appId);
        if (steamData == null) return NotFound();

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

    [HttpGet("{appId}/ccu")]
    public async Task<IActionResult> GetCcu(int appId)
    {
        var count = await steam.GetCurrentPlayersAsync(appId);
        return Ok(new { appId, playerCount = count });
    }

    [HttpGet("{appId}/reviews")]
    public async Task<IActionResult> GetReviews(int appId)
    {
        var reviews = await steam.GetReviewsAsync(appId);
        if (reviews == null) return NotFound();
        return Ok(reviews);
    }
}
