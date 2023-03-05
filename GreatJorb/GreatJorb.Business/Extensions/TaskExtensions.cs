namespace GreatJorb.Business.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> HandleError<T>(this Task<T> task, Func<Exception, Task<T>> handleError)
        {
            try
            {
                return await task;
            }
            catch(Exception e)
            {
                return await handleError(e);
            }
        }

        public static async Task<T?> DefaultIfError<T>(this Task<T> task)
        {
            try
            {
                return await task;
            }
            catch
            {
                return default(T);
            }
        }

        public static async Task IgnoreCancellationException(this Task task)
        {
            try
            {
                await task;
            }
            catch(TaskCanceledException)
            {

            }
        }

        public static async Task IgnoreCancellationException<T>(this Task<T> task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {

            }
        }

        public static async Task<T> IgnoreCancellationException<T>(this Task<T> task, T defaultReturn)
        {
            try
            {
                return await task;
            }
            catch (TaskCanceledException)
            {
                return defaultReturn;
            }
        }

        public static async Task<T?> NotifyError<T>(this Task<T> task, IPage page, IMediator mediator)
        {
            try
            {
                return await task;
            }
            catch(Exception e)
            {
                await mediator.Publish(new BrowserPageChanged(page, page.Url, BrowserAction.FatalError, e));
                return default;
            }
        }

        public static async Task<T> NotifyError<T>(this Task<T> task, IPage page, IMediator mediator, T errorReturn)
        {
            try
            {
                return await task;
            }
            catch (Exception e)
            {
                await mediator.Publish(new BrowserPageChanged(page, page.Url, BrowserAction.FatalError, e));
                return errorReturn;
            }
        }

        public static Task<T> AsTaskResult<T>(this T value) => Task.FromResult(value);


        public static async Task<T> WithMinimumDelay<T>(this Task<T> task, TimeSpan minimumDelay)
        {
            DateTime requestStart = DateTime.Now;
            var result = await task;

            TimeSpan remainingDelay = minimumDelay - (DateTime.Now - requestStart);
            if (remainingDelay.TotalMilliseconds > 0)
                await Task.Delay(remainingDelay);

            return result;
        }

        public static async Task<T?> ThrowNavigationErrorIfNull<T>(this Task<T?> task, IPage page, IMediator mediator, string elementDescription, CancellationToken cancellationToken)
            where T : class
        {
            var result = await task;
            if(result == null)
            {
                var error = new NullReferenceException($"{elementDescription} was not found");

                await mediator.Publish(new BrowserPageChanged(
                    page,
                    page.Url,
                    BrowserAction.FatalError,
                    error));

                throw error;
            }

            return result;

        }
    }
}
