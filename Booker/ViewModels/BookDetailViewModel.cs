using Booker.Models;
using Booker.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;

namespace Booker.ViewModels;

[QueryProperty(nameof(Book), "SavedBook")]
public partial class BookDetailViewModel: ObservableObject
{
    private readonly DataBaseServices _db;
    private IDispatcherTimer _timer;
    private DateTime _startUpdate;
    
    [ObservableProperty]
    private SavedBook? _book;

    [ObservableProperty]
    private TimeSpan _elapsed; // To make timer see seconds

    [ObservableProperty]
    private bool _isRunning;

    public BookDetailViewModel(DataBaseServices session)
    {
        _db = session ; 
        
        // Here we set up the timer logic
        _timer = Application.Current!.Dispatcher.CreateTimer(); // For working with UI
        
        _timer.Interval = TimeSpan.FromSeconds(1); // It will tick every second
        _timer.Tick += Timer_Tick; // Each second we'll call the Timer_Tick
    }
    private void Timer_Tick(object? sender, EventArgs e)
    {
        Elapsed = DateTime.Now - _startUpdate; // It will be updating time on the screen
    }

    // Command to start reading timer
    [RelayCommand]
    private void StartTimer()
    {
        if (IsRunning) return;
        
        _startUpdate = DateTime.Now;
        
        IsRunning = true;
        
        _timer.Start();
    }
    
    // Command for stopping the timer
    [RelayCommand]
    private async Task StopTimerAsync()
    {
        if (IsRunning == false) return;
        
        _timer.Stop();
        IsRunning = false;
        
        string? result = await Shell.Current.DisplayPromptAsync(
            "Good job! 🎉", 
            $"What page did you stop at? (Current: {Book!.PagesRead})",
            initialValue: Book.PagesRead.ToString(),
            keyboard: Keyboard.Numeric);
        
        if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int currentPage))
        {
            if (currentPage <= Book.PagesRead || (Book.PageCount > 0 && currentPage > Book.PageCount))
            {
                await Shell.Current.DisplayAlert("Hold on", "Page number must be greater than your current progress and within the total page count.", "OK");
                return;
            }

            int pagesReadInSession = currentPage - Book.PagesRead;

            var newSession = new ReadingSession()
            {
                BookId = Book!.Id,
                StartDateTime = _startUpdate,
                EndDateTime = DateTime.Now,
                PagesRead = pagesReadInSession
            };
            
            await _db.SaveReadingSessionAsync(newSession);

            Book.PagesRead = currentPage;
            Book.IsFinished = Book.PageCount > 0 && currentPage >= Book.PageCount;

            await _db.UpdateBookAsync(Book);

            OnPropertyChanged(nameof(Book));
            
            await LoadStatisticsAsync();
        }
    }
    
    [RelayCommand]
    private async Task UpdateProgressAsync()
    {
        if (Book == null) return; 

        string? input = await Shell.Current.DisplayPromptAsync(
            "Updating Progress",
            $"How many pages have you read (max {Book.PageCount}):",
            initialValue: Book.PagesRead.ToString(),
            keyboard: Keyboard.Numeric);

        if (input == null) return; 

        if (!int.TryParse(input, out int pages) || pages < 0 || (Book.PageCount > 0 && pages > Book.PageCount))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter pages count.", "Ok");
            return;
        }

        Book.PagesRead = pages;
        Book.IsFinished = Book.PageCount > 0 && pages >= Book.PageCount;

        await _db.UpdateBookAsync(Book);

        OnPropertyChanged(nameof(Book)); 
    }
    
    [ObservableProperty]
    private bool _hasStats;

    [ObservableProperty]
    private Chart? _statsChart;
    
    [ObservableProperty]
    private ObservableCollection<ReadingSession> _readingSessions = new();
    
    partial void OnBookChanged(SavedBook? value)
    {
        if (value != null)
        {
            _ = LoadStatisticsAsync(); 
        }
    }

    private async Task LoadStatisticsAsync()
    {
        var sessions = await _db.GetReadingSessionsAsync(Book!.Id);
        ReadingSessions = new ObservableCollection<ReadingSession>(
            sessions.OrderByDescending(s => s.StartDateTime)
        );

        var groupedStats = sessions
            .GroupBy(s => s.StartDateTime.Date)
            .Select(g => new { Date = g.Key, Pages = g.Sum(s => s.PagesRead) })
            .OrderBy(g => g.Date)
            .ToList();

        if (groupedStats.Count == 0)
        {
            HasStats = false;
            return;
        }

        HasStats = true;

        var chartEntries = groupedStats.Select(stat => new ChartEntry(stat.Pages)
        {
            Label = stat.Date.ToString("dd MMM"),
            ValueLabel = stat.Pages.ToString(),
            Color = SKColor.Parse("#8FBC8F"),
            TextColor = SKColor.Parse("#7A9E7E"),
            ValueLabelColor = SKColor.Parse("#F0EDE4")
        }).ToList();

        StatsChart = new LineChart
        {
            Entries = chartEntries,
            LabelTextSize = 35,
            BackgroundColor = SKColors.Transparent,
            LabelColor = SKColor.Parse("#7A9E7E"),
            Margin = 20
        };
    }
}