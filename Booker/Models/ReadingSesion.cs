using SQLite;

namespace Booker.Models;

public class ReadingSession
{
    [PrimaryKey, AutoIncrement]
    public int SesionId { get; set; }
    
    [Indexed]
    public int BookId { get; set; }
    
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    
    [Ignore] 
    public TimeSpan Duration => EndDateTime - StartDateTime;

    public int PagesRead { get; set; }
    
}