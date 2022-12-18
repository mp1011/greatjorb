namespace GreatJorb.Business.Services.LocalStore;

public class ContextLock : IDisposable
{
    private static readonly TimeSpan _maxWaitTime = TimeSpan.FromSeconds(10);

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);
    private bool disposedValue;

    public bool IsReleased { get; private set; }    

    public bool HasClaimedLock { get; private set; }

    public DateTime LockRequestTime { get; }
    public DateTime? LockReleaseTime { get; private set; }

    private ContextLock()
    {
        LockRequestTime = DateTime.Now;
    }

    public static async Task<ContextLock> AcquireAsync()
    {
        try
        {
            ContextLock contextLock = new();

            bool success = false;
            while(!success && contextLock.LockRequestTime.TimeSince() <= _maxWaitTime)
            {
                success = await _semaphore.WaitAsync(TimeSpan.FromSeconds(1));

                contextLock.HasClaimedLock = success;
            }

            if(!success)
            {
                throw new UnableToAcquireContextLockException();
            }

            return contextLock;
        }
        catch (UnableToAcquireContextLockException)
        {
            throw;
        }
        catch(Exception e) 
        {
            throw new UnableToAcquireContextLockException(e);
        }
    }

    private void Release()
    {
        try
        {
            if (IsReleased || !HasClaimedLock)
                return;

            _semaphore.Release();
            IsReleased = true;
            LockReleaseTime = DateTime.Now;
        }
        catch(Exception e)
        {
            throw new Exception("Failed to release Context Lock", e);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Release();
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
