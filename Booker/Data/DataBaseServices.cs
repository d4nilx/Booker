using SQLite;
using Booker.Models;

namespace Booker.Data;

public class DataBaseServices
{
    private SQLiteAsyncConnection? _db;

    // SemaphoreSlim(1,1) acts as an async-compatible mutex.
    // Without this, two ViewModels calling Init() simultaneously on first launch could
    // both see _db == null and create two connections, causing data corruption.
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private async Task Init()
    {
        if (_db != null) return;

        await _initLock.WaitAsync();
        try
        {
            // Double-check after acquiring lock in case another caller already initialised.
            if (_db != null) return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "booker.db");
            _db = new SQLiteAsyncConnection(databasePath);
            await _db.CreateTableAsync<SavedBook>();
        }
        finally
        {
            _initLock.Release();
        }
    }

    public async Task<List<SavedBook>> GetSavedBooks()
    {
        await Init();
        return await _db!.Table<SavedBook>().ToListAsync();
    }

    public async Task<int> SaveBookAsync(SavedBook book)
    {
        await Init();
        return await _db!.InsertAsync(book);
    }

    public async Task<int> DeleteBookAsync(SavedBook book)
    {
        await Init();
        return await _db!.DeleteAsync(book);
    }

    public async Task<int> UpdateBookAsync(SavedBook book)
    {
        await Init();
        return await _db!.UpdateAsync(book);
    }
}
