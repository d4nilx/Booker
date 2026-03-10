using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Booker.Models;
using Booker.Data;

namespace Booker.ViewModels;

public class GenreStat
{
    public string GenreName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public partial class StatsViewModel : ObservableObject
{
    private readonly DataBaseServices _dbService;

    [ObservableProperty] private int _totalBooks;
    [ObservableProperty] private int _readBooks;
    [ObservableProperty] private double _readPercentage;
    [ObservableProperty] private int _totalPagesRead;

    public ObservableCollection<GenreStat> TopGenres { get; } = new();

    public StatsViewModel(DataBaseServices dbService)
    {
        _dbService = dbService;
        _ = LoadStatisticsAsync();
    }

    private async Task LoadStatisticsAsync()
    {
        var books = await _dbService.GetSavedBooks();
        if (books == null || books.Count == 0) return;

        var genreGroups = books
            .Where(b => !string.IsNullOrEmpty(b.Category))
            .GroupBy(b => b.Category)
            .Select(g => new GenreStat { GenreName = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(5)
            .ToList();

        // All UI-bound property writes must happen on the main thread.
        // Previously the collection and properties were set from a background Task.Run thread.
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            TotalBooks = books.Count;
            ReadBooks = books.Count(b => b.IsFinished);
            ReadPercentage = (double)ReadBooks / TotalBooks;
            TotalPagesRead = books.Sum(b => b.PagesRead);

            TopGenres.Clear();
            foreach (var genre in genreGroups)
                TopGenres.Add(genre);
        });
    }
}
