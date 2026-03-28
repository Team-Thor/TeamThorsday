namespace Shared.DTOs;

public class SteamReviewsDto
{
    public string ReviewScoreDesc { get; set; } = ""; // e.g. "Very Positive"
    public int TotalPositive { get; set; }
    public int TotalNegative { get; set; }
    public int TotalReviews { get; set; }
    public List<SteamReviewDto> Reviews { get; set; } = new();
}

public class SteamReviewDto
{
    public bool VotedUp { get; set; }
    public int PlaytimeAtReview { get; set; } // in minutes
    public string Review { get; set; } = "";
}
