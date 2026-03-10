# 📚 Booker

A personal reading tracker built with **.NET MAUI**, targeting Android, iOS, and macOS.

Search for books using the Google Books API, save them to your local library, track your reading progress, and view your reading stats — all in one place.

---

## Features

- 🔍 **Search** — Find books via the Google Books API
- 📚 **Library** — Save books and track reading progress page by page
- 🏠 **Home** — See your currently reading book at a glance
- 📊 **Stats** — Total books, pages read, completion rate, and top genres

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET MAUI (net10.0) |
| Architecture | MVVM |
| Local database | SQLite via `sqlite-net-pcl` |
| MVVM helpers | CommunityToolkit.Mvvm |
| Book data | Google Books API (free, no key required) |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Rider](https://www.jetbrains.com/rider/) or [Visual Studio 2022+](https://visualstudio.microsoft.com/) with MAUI workload

### Run

```bash
git clone https://github.com/YOUR_USERNAME/Booker.git
cd Booker
dotnet restore
dotnet build
```

Then run on your target platform from your IDE, or:

```bash
# Android
dotnet run -f net10.0-android

# iOS (Mac only)
dotnet run -f net10.0-ios

# macOS
dotnet run -f net10.0-maccatalyst
```

---

## Project Structure

```
Booker/
├── Models/          # Data models (SavedBook, GoogleBooksResponse)
├── ViewModels/      # MVVM ViewModels (CommunityToolkit.Mvvm)
├── Views/           # XAML pages
├── Services/        # BookService (Google Books API)
├── Data/            # DataBaseServices (SQLite)
├── Converters/      # XAML value converters
└── Platforms/       # Platform-specific code (Android, iOS, macOS)
```

---

## Notes

- Book data is stored **locally on-device** via SQLite — no account or backend needed.
- The Google Books API is used without an API key (public endpoint, rate-limited).
- This is a personal/learning project — not yet production-ready.

---

## License

MIT
