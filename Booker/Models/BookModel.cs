using System.Collections.Generic; 
using System.Text.Json.Serialization;

namespace Booker.Models;

public class GoogleBooksResponse
{
    [JsonPropertyName("items")]
    public List<BooksResult> Items { get; set; }
}

public class BooksResult
{
    [JsonPropertyName("volumeInfo")]
    public VolumeInfo VolumeInfo { get; set; }
}

public class VolumeInfo
{
    [JsonPropertyName("title")] 
    public string Title { get; set; }
  
    [JsonPropertyName("authors")] 
    public List<string> Authors { get; set; }
    
    [JsonPropertyName("pageCount")] 
    public int PageCount { get; set; }
    
    [JsonPropertyName("categories")] 
    public List<string> Categories { get; set; }
    
    [JsonPropertyName("imageLinks")] 
    public ImageLinks ImageLinks { get; set; }
    
    [JsonPropertyName("language")] 
    public string Language { get; set; }
}

public class ImageLinks
{
    [JsonPropertyName("thumbnail")] 
    public string Thumbnail { get; set; } 
}