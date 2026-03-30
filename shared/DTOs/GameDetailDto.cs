namespace Shared.DTOs;

// combined steamspy + cheapshark data for a single game
public class GameDetailDto
{
    public int AppId { get; set; }
    public string Name { get; set; } = "";
    public string Developer { get; set; } = "";
    public string Publisher { get; set; } = "";
    public string Owners { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Positive { get; set; }
    public int Negative { get; set; }
    public int AveragePlaytimeMinutes { get; set; }
    public Dictionary<string, int> Tags { get; set; } = new();
    public List<CheapSharkDealDto> Deals { get; set; } = new();
    public string ImageUrl => $"https://cdn.akamai.steamstatic.com/steam/apps/{AppId}/header.jpg";
}
