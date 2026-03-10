using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Booker.Models;
using Booker.Data;

namespace Booker.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DataBaseServices _dbService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCurrentBook))]
    [NotifyPropertyChangedFor(nameof(ReadingProgress))]
    private SavedBook? _currentBook;

    // HasCurrentBook drives visibility of the "currently reading" card vs empty-state card.
    public bool HasCurrentBook => CurrentBook != null;

    // ReadingProgress is a 0.0–1.0 value for the ProgressBar.
    // Computed from CurrentBook so it updates whenever CurrentBook changes.
    public double ReadingProgress =>
        CurrentBook is { PageCount: > 0 }
            ? Math.Clamp((double)CurrentBook.PagesRead / CurrentBook.PageCount, 0, 1)
            : 0;

    public MainViewModel(DataBaseServices dbService)
    {
        _dbService = dbService;
        _ = LoadMainPageDataAsync();
    }

    public async Task LoadMainPageDataAsync()
    {
        var books = await _dbService.GetSavedBooks();

        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            // Show the first unfinished book as "currently reading".
            CurrentBook = books.FirstOrDefault(b => !b.IsFinished);
        });
    }

    // Navigates to the Library tab using Shell's URI routing.
    // Previously this was a dead binding — the command didn't exist.
    [RelayCommand]
    private async Task GoToLibrary()
    {
        await Shell.Current.GoToAsync("//LibraryPage");
    }

    // Navigates to the Search tab.
    [RelayCommand]
    private async Task GoToSearch()
    {
        await Shell.Current.GoToAsync("//SearchPage");
    }
}
