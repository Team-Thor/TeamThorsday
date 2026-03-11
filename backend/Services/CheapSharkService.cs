using System.Text.Json;
using Backend.Models;
using Shared.DTOs;

namespace Backend.Services;

public class CheapSharkService(HttpClient http)
{
    private const string BaseUrl = "https://www.cheapshark.com/api/1.0";

    private static readonly Dictionary<string, object> Cache = new();
    private static readonly Dictionary<string, DateTime> CacheTime = new();

    // store name lookup, loaded once
    private static Dictionary<string, string> _storeNames = new();

    // get deals for a specific steam app
    public async Task<List<CheapSharkDealDto>> GetDealsByAppIdAsync(string steamAppId)
    {
        var key = $"deals_{steamAppId}";

        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (List<CheapSharkDealDto>)Cache[key];

        // ensure store names are loaded
        await LoadStoresAsync();

        var json = await http.GetStringAsync($"{BaseUrl}/deals?steamAppID={steamAppId}&pageSize=10");
        var raw = JsonSerializer.Deserialize<List<CheapSharkDeal>>(json);

        if (raw == null) return new List<CheapSharkDealDto>();

        var result = new List<CheapSharkDealDto>();
        foreach (var d in raw)
        {
            decimal.TryParse(d.SalePrice, out var sale);
            decimal.TryParse(d.NormalPrice, out var normal);
            decimal.TryParse(d.Savings, out var savings);

            var storeName = _storeNames.ContainsKey(d.StoreId) ? _storeNames[d.StoreId] : $"Store {d.StoreId}";

            result.Add(new CheapSharkDealDto
            {
                DealId = d.DealId,
                StoreName = storeName,
                SalePrice = sale,
                NormalPrice = normal,
                SavingsPercent = Math.Round(savings, 1),
                IsOnSale = d.IsOnSale == "1",
                Thumb = d.Thumb
            });
        }

        Cache[key] = result;
        CacheTime[key] = DateTime.Now;
        return result;
    }

    // cache id to store name
    private async Task LoadStoresAsync()
    {
        if (_storeNames.Count > 0) return;

        var json = await http.GetStringAsync($"{BaseUrl}/stores");
        var stores = JsonSerializer.Deserialize<List<CheapSharkStore>>(json);

        if (stores == null) return;

        foreach (var s in stores)
        {
            if (s.IsActive == 1)
                _storeNames[s.StoreId] = s.StoreName;
        }
    }
}
