using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Booker.Data;
using Booker.Services;
using Booker.ViewModels;
using Booker.Views;
using Microcharts.Maui;
using System.Reflection;

namespace Booker;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Booker.appsettings.json");
        if (stream != null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(config);
        }
        builder
            .UseMauiApp<App>()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // --- Services ---
        builder.Services.AddSingleton<DataBaseServices>();
        
        builder.Services.AddSingleton<HttpClient>();
        builder.Services.AddSingleton<BookService>();

        // --- ViewModels ---
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<LibraryViewModel>();
        builder.Services.AddTransient<StatsViewModel>();
        builder.Services.AddTransient<BookDetailViewModel>();

        // --- Pages ---
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<LibraryPage>();
        builder.Services.AddTransient<StatsPage>();
        builder.Services.AddTransient<BookDetailPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
