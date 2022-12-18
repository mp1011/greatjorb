namespace GreatJorb.Business.Services.LocalStore;

public class LocalDataContextProvider
{
    private readonly ISettingsService _settingsService;

    public LocalDataContextProvider(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public LocalDataContext GetContext() => new LocalDataContext(_settingsService);
}

public class LocalDataContext : IDisposable
{
    private bool disposedValue;

    private LiteDatabase _liteDatabase;

    public LocalDataContext(ISettingsService settingsService)
    {
        _liteDatabase = new LiteDatabase(settingsService.LocalStoragePath);
    }

    public async Task Store<T>(T item)
        where T:ILocalStorable
    {
        using var contextLock = await ContextLock.AcquireAsync();
        _liteDatabase.GetCollection<T>().Insert(item);
    }

    public async Task<T?> Retrieve<T>(string key)
        where T : ILocalStorable
    {
        using var contextLock = await ContextLock.AcquireAsync();

        return RetrieveWithoutLocking<T>(key);
    }

    private T? RetrieveWithoutLocking<T>(string key)
      where T : ILocalStorable
    {
        return _liteDatabase.GetCollection<T>()
            .Query()
            .Where(p => p.StorageKey == key)
            .FirstOrDefault();
    }

    public async Task<T[]> Retrieve<T>(IEnumerable<string> keys)
       where T : ILocalStorable
    {
        List<T> results = new();

        using var contextLock = await ContextLock.AcquireAsync();

        foreach(var key in keys)
        {
            var retrieved = RetrieveWithoutLocking<T>(key);
            if (retrieved != null)
                results.Add(retrieved);
        }

        return results.ToArray();
    }

    public async Task<T[]> Retrieve<T>(Expression<Func<T,bool>> filter)
        where T : ILocalStorable
    {
        using var contextLock = await ContextLock.AcquireAsync();

        return _liteDatabase.GetCollection<T>()
            .Query()
            .Where(filter)
            .ToArray();
    }

    public async Task Remove<T>(string key)
      where T : ILocalStorable
    {
        using var contextLock = await ContextLock.AcquireAsync();

        _liteDatabase.GetCollection<T>()
            .DeleteMany(p => p.StorageKey == key);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _liteDatabase.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
