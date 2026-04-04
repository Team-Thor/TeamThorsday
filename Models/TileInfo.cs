using Team_Thorsday.Models;

public class TileInfo
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string ValueHeader { get; set; } = "Value";

    public List<TileItem> PreviewItems { get; set; } = new();
    public List<TileItem> FullItems { get; set; } = new();
    public bool IsLoading { get; set; } = true;
    public string? ErrorMessage { get; set; }
}
