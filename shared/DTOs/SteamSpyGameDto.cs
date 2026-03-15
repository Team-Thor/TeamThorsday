namespace Shared.DTOs;

// used by both the backend and the blazor client
public class SteamSpyGameDto
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
    public int PeakConcurrentUsersYesterday { get; set; }
    public string Price { get; set; } = ""; // in cents e.g. "1999" = $19.99
    public Dictionary<string, int> Tags { get; set; } = new();
}
