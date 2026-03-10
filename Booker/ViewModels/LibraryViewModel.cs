using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Booker.Models;
using Booker.Data;

namespace Booker.ViewModels;

public partial class LibraryViewModel : ObservableObject
{
    private readonly DataBaseServices _dbService;

    // IsRefreshing drives the RefreshView spinner in LibraryPage.xaml
    [ObservableProperty]
    private bool _isRefreshing;

    public ObservableCollection<SavedBook> MyLibrary { get; } = new();

    public LibraryViewModel(DataBaseServices dbService)
    {
        _dbService = dbService;
        // Fire-and-forget is acceptable here for initial load, but we must marshal
        // all collection/UI updates back onto the main thread (see LoadBooksAsync).
        _ = LoadBooksAsync();
    }

    private async Task LoadBooksAsync()
    {
        var books = await _dbService.GetSavedBooks();

        // ObservableCollection must only be modified on the UI thread.
        // Previously this was missing, causing intermittent crashes on startup.
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            MyLibrary.Clear();
            foreach (var book in books)
                MyLibrary.Add(book);
        });
    }

    // RefreshCommand is bound to the RefreshView in LibraryPage.xaml
    [RelayCommand]
    private async Task Refresh()
    {
        IsRefreshing = true;
        await LoadBooksAsync();
        IsRefreshing = false;
    }

    // DeleteBookCommand is bound to the 🗑 button; CommandParameter passes the book.
    [RelayCommand]
    private async Task DeleteBook(SavedBook book)
    {
        if (book == null) return;

        bool confirmed = await Shell.Current.DisplayAlert(
            "Delete",
            $"Remove \"{book.Title}\" from your library?",
            "Delete", "Cancel");

        if (!confirmed) return;

        await _dbService.DeleteBookAsync(book);

        // Update the in-memory list immediately — no need to reload from DB.
        await MainThread.InvokeOnMainThreadAsync(() => MyLibrary.Remove(book));
    }

    // UpdateProgressCommand is bound to the "Update" button; CommandParameter passes the book.
    [RelayCommand]
    private async Task UpdateProgress(SavedBook book)
    {
        if (book == null) return;

        string? input = await Shell.Current.DisplayPromptAsync(
            "Update Progress",
            $"Pages read for \"{book.Title}\" (max {book.PageCount}):",
            initialValue: book.PagesRead.ToString(),
            keyboard: Keyboard.Numeric);

        if (input == null) return; // user cancelled

        if (!int.TryParse(input, out int pages) || pages < 0 || (book.PageCount > 0 && pages > book.PageCount))
        {
            await Shell.Current.DisplayAlert("Invalid", "Please enter a valid page number.", "OK");
            return;
        }

        book.PagesRead = pages;
        // Automatically mark finished when the user reaches the last page.
        book.IsFinished = book.PageCount > 0 && pages >= book.PageCount;

        await _dbService.UpdateBookAsync(book);

        // Refresh the list so the progress bar and "Finished" label update instantly.
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            int idx = MyLibrary.IndexOf(book);
            if (idx >= 0)
            {
                MyLibrary.RemoveAt(idx);
                MyLibrary.Insert(idx, book);
            }
        });
    }
}
