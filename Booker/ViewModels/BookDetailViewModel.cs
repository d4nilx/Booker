using Booker.Models;
using Booker.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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
        
        // Here we make a logic for a timer
        _timer = Application.Current!.Dispatcher.CreateTimer(); // For working wit UI
        
        _timer.Interval = TimeSpan.FromSeconds(1); // It wiil tick every second
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
    
    // Command for stoping the timer 
    [RelayCommand]
    private async Task StopTimerAsync()
    {
        if (IsRunning ==  false) return;
        
        _timer.Stop();
        IsRunning = false;
        
        // End of timer will call for an question how many pages user've read 
        
        string result = await Shell.Current.DisplayPromptAsync(
        "Good job!", 
            "How many have you read!",
            keyboard: Keyboard.Numeric);
        
        // Here we just check in case user choose cancel
        if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int pages))
        {
            var newSession = new ReadingSession()
            {
                BookId = Book!.Id,
                StartDateTime = _startUpdate,
                EndDateTime = DateTime.Now,
                PagesRead = pages
            };
                await _db.SaveReadingSessionAsync(newSession);
        }
    }
}