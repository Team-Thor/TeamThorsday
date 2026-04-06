using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

/// <summary>
/// API Controller for direct SteamSpy data access.
/// Provides raw endpoints to SteamSpy without combining with other services.
/// Route prefix: /api/steamspy
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SteamSpyController(
    SteamSpyService steamSpy)      // Injected SteamSpy service via primary constructor
    : ControllerBase
{
    /// <summary>
    /// GET /api/steamspy/tag/{tag}
    /// Returns a list of games that match the specified SteamSpy tag.
    /// Examples: "Roguelite", "FPS", "Indie", "Co-op", etc.
    /// </summary>
    [HttpGet("tag/{tag}")]
    public async Task<IActionResult> GetByTag(string tag)
    {
        // Delegate the request to the SteamSpyService
        var games = await steamSpy.GetGamesByTagAsync(tag);
        
        return Ok(games);   // Returns 200 OK with List<SteamSpyGameDto>
    }

    /// <summary>
    /// GET /api/steamspy/app/{appId}
    /// Returns detailed information for a single Steam app/game by its App ID.
    /// Example: /api/steamspy/app/440 → Returns data for Team Fortress 2
    /// </summary>
    [HttpGet("app/{appId}")]
    public async Task<IActionResult> GetApp(int appId)
    {
        // Fetch detailed game/app data from SteamSpy
        var game = await steamSpy.GetAppDetailsAsync(appId);

        // Return 404 Not Found if the app does not exist in SteamSpy
        if (game == null) 
            return NotFound();

        return Ok(game);   // Returns 200 OK with SteamSpyGameDto
    }
}