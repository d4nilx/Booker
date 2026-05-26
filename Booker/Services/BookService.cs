using System.Text.Json;
using Booker.Models;
using Microsoft.Extensions.Configuration;

namespace Booker.Services;

public class BookService
{
    private readonly HttpClient _httpClient;

    // HttpClient is injected by DI (registered as singleton in MauiProgram).
    // This avoids socket exhaustion that comes from creating a new HttpClient per call.
    private readonly string _apiKey;

    public BookService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["GoogleBooksApiKey"] ?? string.Empty;
    }

    public async Task<GoogleBooksResponse?> SearchBooksAsync(string searchQuery)
    {
        // Uri.EscapeDataString encodes spaces and special characters properly.
        // Without this, a query like "the great gatsby" sends a malformed URL and returns nothing.
        var encoded = Uri.EscapeDataString(searchQuery);
        string url = $"...&key={_apiKey}";

        try
        {
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string jsonText = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<GoogleBooksResponse>(jsonText);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BookService error: {ex.Message}");
        }

        return null;
    }
}
