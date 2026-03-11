namespace Shared.DTOs;

public class CheapSharkDealDto
{
    public string DealId { get; set; } = "";
    public string StoreName { get; set; } = "";
    public decimal SalePrice { get; set; }
    public decimal NormalPrice { get; set; }
    public decimal SavingsPercent { get; set; }
    public bool IsOnSale { get; set; }
    public string Thumb { get; set; } = "";
}
