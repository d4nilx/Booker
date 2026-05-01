using SQLite;

namespace Booker.Models;

public class SavedBook
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Thumbnail { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Category { get; set; } = string.Empty;
    public int PagesRead { get; set; }
    public bool IsFinished { get; set; }

    
    [Ignore] 
    public double ReadingProgress =>
        PageCount > 0 ? Math.Clamp((double)PagesRead / PageCount, 0.0, 1.0) : 0;
}
