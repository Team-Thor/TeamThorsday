using System.Text.Json;
using Backend.Models;
using Shared.DTOs;

namespace Backend.Services;

/// <summary>
/// Service responsible for fetching current game deals from the CheapShark API.
/// Includes caching and store name resolution for better performance and UX.
/// </summary>
public class CheapSharkService(HttpClient http)
{
    private const string BaseUrl = "https://www.cheapshark.com/api/1.0";

    /// <summary>
    /// In-memory cache for deal results (keyed by steamAppId).
    /// </summary>
    private static readonly Dictionary<string, object> Cache = new();

    /// <summary>
    /// Tracks when each cached entry was created (used for expiration).
    /// </summary>
    private static readonly Dictionary<string, DateTime> CacheTime = new();

    /// <summary>
    /// Static cache mapping store IDs to human-readable store names (e.g., "Steam", "Epic Games Store").
    /// Loaded once on first use.
    /// </summary>
    private static Dictionary<string, string> _storeNames = new();

    /// <summary>
    /// Gets current deals for a specific Steam App ID from CheapShark.
    /// Results are cached for 30 minutes to reduce API calls.
    /// </summary>
    public async Task<List<CheapSharkDealDto>> GetDealsByAppIdAsync(string steamAppId)
    {
        var key = $"deals_{steamAppId}";

        // Return cached result if still valid (within 30 minutes)
        if (Cache.ContainsKey(key) && DateTime.Now - CacheTime[key] < TimeSpan.FromMinutes(30))
            return (List<CheapSharkDealDto>)Cache[key];

        // Ensure store names are loaded before processing deals
        await LoadStoresAsync();

        // Fetch deals from CheapShark API
        var json = await http.GetStringAsync($"{BaseUrl}/deals?steamAppID={steamAppId}&pageSize=10");
        var raw = JsonSerializer.Deserialize<List<CheapSharkDeal>>(json);

        if (raw == null) 
            return new List<CheapSharkDealDto>();

        var result = new List<CheapSharkDealDto>();

        foreach (var d in raw)
        {
            decimal.TryParse(d.SalePrice, out var sale);
            decimal.TryParse(d.NormalPrice, out var normal);
            decimal.TryParse(d.Savings, out var savings);

            // Resolve store ID to friendly name (fallback if unknown)
            var storeName = _storeNames.ContainsKey(d.StoreId) 
                ? _storeNames[d.StoreId] 
                : $"Store {d.StoreId}";

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

        // Cache the processed result and update timestamp
        Cache[key] = result;
        CacheTime[key] = DateTime.Now;

        return result;
    }

    /// <summary>
    /// Loads all active store names from CheapShark and caches them in a dictionary.
    /// This is called once per application lifetime (static cache).
    /// </summary>
    private async Task LoadStoresAsync()
    {
        // Skip if stores have already been loaded
        if (_storeNames.Count > 0) 
            return;

        var json = await http.GetStringAsync($"{BaseUrl}/stores");
        var stores = JsonSerializer.Deserialize<List<CheapSharkStore>>(json);

        if (stores == null) 
            return;

        foreach (var s in stores)
        {
            if (s.IsActive == 1)
                _storeNames[s.StoreId] = s.StoreName;
        }
    }
}