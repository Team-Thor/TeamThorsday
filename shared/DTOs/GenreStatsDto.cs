namespace Shared.DTOs;

public class GenreStatsDto
{
    public string Genre { get; set; } = "";
    public int GameCount { get; set; }
    public double AvgReviewScore { get; set; } // 0-100
    public double AvgPriceUsd { get; set; }
    public int AvgPlaytimeMinutes { get; set; }
    public int TotalCcu { get; set; }
}
