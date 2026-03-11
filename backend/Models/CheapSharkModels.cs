using System.Text.Json.Serialization;

namespace Backend.Models;

public class CheapSharkDeal
{
    [JsonPropertyName("dealID")]
    public string DealId { get; set; } = "";

    [JsonPropertyName("storeID")]
    public string StoreId { get; set; } = "";

    [JsonPropertyName("salePrice")]
    public string SalePrice { get; set; } = "";

    [JsonPropertyName("normalPrice")]
    public string NormalPrice { get; set; } = "";

    [JsonPropertyName("savings")]
    public string Savings { get; set; } = "";

    [JsonPropertyName("isOnSale")]
    public string IsOnSale { get; set; } = "";

    [JsonPropertyName("thumb")]
    public string Thumb { get; set; } = "";
}

public class CheapSharkStore
{
    [JsonPropertyName("storeID")]
    public string StoreId { get; set; } = "";

    [JsonPropertyName("storeName")]
    public string StoreName { get; set; } = "";

    [JsonPropertyName("isActive")]
    public int IsActive { get; set; }
}
