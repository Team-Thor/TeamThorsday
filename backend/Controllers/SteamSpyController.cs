using Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SteamSpyController(SteamSpyService steamSpy) : ControllerBase
{
    // GET /api/steamspy/tag/Roguelite
    [HttpGet("tag/{tag}")]
    public async Task<IActionResult> GetByTag(string tag)
    {
        var games = await steamSpy.GetGamesByTagAsync(tag);
        return Ok(games);
    }

    // GET /api/steamspy/app/440
    [HttpGet("app/{appId}")]
    public async Task<IActionResult> GetApp(int appId)
    {
        var game = await steamSpy.GetAppDetailsAsync(appId);
        if (game == null) return NotFound();
        return Ok(game);
    }
}
