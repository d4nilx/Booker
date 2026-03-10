using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Booker.Models;
using Booker.Services;
using Booker.Data;

namespace Booker.ViewModels;

// ObservableObject (from CommunityToolkit) replaces BindableObject + manual OnPropertyChanged().
// [ObservableProperty] auto-generates the private field, public property, and change notification.
public partial class SearchViewModel : ObservableObject
{
    private readonly BookService _bookService;
    private readonly DataBaseServices _dbService;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    // IsBusy drives the ActivityIndicator visibility in SearchPage.xaml
    [ObservableProperty]
    private bool _isBusy;

    // The collection was previously typed as SavedBook but the API returns VolumeInfo data.
    // We map VolumeInfo → SavedBook so the same model flows through to the DB on save.
    public ObservableCollection<SavedBook> BooksList { get; } = new();

    public SearchViewModel(BookService bookService, DataBaseServices dbService)
    {
        _bookService = bookService;
        _dbService = dbService;
    }

    // Named SearchAsync so [RelayCommand] generates exactly "SearchCommand" to match the XAML binding.
    // [RelayCommand] derives the command name from the method name by stripping "Async" → "SearchCommand".
    [RelayCommand]
    private async Task SearchAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery)) return;

        IsBusy = true;
        BooksList.Clear();

        try
        {
            var result = await _bookService.SearchBooksAsync(SearchQuery);

            if (result?.Items != null)
            {
                foreach (var item in result.Items)
                {
                    var info = item.VolumeInfo;
                    if (info == null) continue;

                    // Map the API VolumeInfo into a SavedBook so the UI and DB use one model.
                    BooksList.Add(new SavedBook
                    {
                        Title = info.Title ?? "Unknown Title",
                        Author = info.Authors != null ? string.Join(", ", info.Authors) : "Unknown Author",
                        Thumbnail = info.ImageLinks?.Thumbnail?.Replace("http://", "https://") ?? string.Empty,
                        PageCount = info.PageCount,
                        Category = info.Categories?.FirstOrDefault() ?? string.Empty
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
        }
        finally
        {
            // Always reset IsBusy even if an exception occurred.
            IsBusy = false;
        }
    }

    // SaveBookCommand is bound to the "+" button on each search result card.
    // CommandParameter passes the specific SavedBook tapped.
    [RelayCommand]
    private async Task SaveBook(SavedBook book)
    {
        if (book == null) return;

        try
        {
            await _dbService.SaveBookAsync(book);
            await Shell.Current.DisplayAlert("Saved", $"\"{book.Title}\" added to your library.", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save error: {ex.Message}");
            await Shell.Current.DisplayAlert("Error", "Could not save the book. Please try again.", "OK");
        }
    }
}
