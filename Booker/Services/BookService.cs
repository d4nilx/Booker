using System.Text.Json;
using Booker.Models;

namespace Booker.Services;

public class BookService
{
    private readonly HttpClient _httpClient;

    // HttpClient is injected by DI (registered as singleton in MauiProgram).
    // This avoids socket exhaustion that comes from creating a new HttpClient per call.
    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GoogleBooksResponse?> SearchBooksAsync(string searchQuery)
    {
        // Uri.EscapeDataString encodes spaces and special characters properly.
        // Without this, a query like "the great gatsby" sends a malformed URL and returns nothing.
        var encoded = Uri.EscapeDataString(searchQuery);
        string url = $"https://www.googleapis.com/books/v1/volumes?q={encoded}&maxResults=20";

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
