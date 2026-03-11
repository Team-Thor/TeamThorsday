using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController(SteamSpyService steamSpy, CheapSharkService cheapShark) : ControllerBase
{
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
}
