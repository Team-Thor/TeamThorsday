public class TileInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty; // e.g., "SteamDB Player Count"
    public string GraphType { get; set; } = "placeholder"; // Future: "line", "bar", "pie" etc.
    
    // Future editable fields (you can expand this later)
    // public string DataSource { get; set; } = "steamdb";
    // public string Metric { get; set; } = "players";
}