using System.Text.Json.Serialization;

namespace Backend.Models;

// full details for a single app (appdetails endpoint)
public class SteamSpyAppDetails
{
    [JsonPropertyName("appid")]
    public int AppId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("developer")]
    public string Developer { get; set; } = "";

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = "";

    [JsonPropertyName("positive")]
    public int Positive { get; set; }

    [JsonPropertyName("negative")]
    public int Negative { get; set; }

    [JsonPropertyName("owners")]
    public string Owners { get; set; } = "";

    [JsonPropertyName("average_forever")]
    public int AverageForever { get; set; }

    [JsonPropertyName("ccu")]
    public int PeakConcurrentUsersYesterday { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; } = "";

    [JsonPropertyName("genre")]
    public string Genre { get; set; } = "";

    // tags come back as {"Tag Name": voteCount, ...}
    [JsonPropertyName("tags")]
    public Dictionary<string, int>? Tags { get; set; }
}

// trimmed version returned by tag/genre/top100 endpoints (no tags or genre fields)
public class SteamSpyBasicApp
{
    [JsonPropertyName("appid")]
    public int AppId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("developer")]
    public string Developer { get; set; } = "";

    [JsonPropertyName("publisher")]
    public string Publisher { get; set; } = "";

    [JsonPropertyName("positive")]
    public int Positive { get; set; }

    [JsonPropertyName("negative")]
    public int Negative { get; set; }

    [JsonPropertyName("owners")]
    public string Owners { get; set; } = "";

    [JsonPropertyName("average_forever")]
    public int AverageForever { get; set; }

    [JsonPropertyName("ccu")]
    public int PeakConcurrentUsersYesterday { get; set; }

    [JsonPropertyName("price")]
    public string Price { get; set; } = "";
}
