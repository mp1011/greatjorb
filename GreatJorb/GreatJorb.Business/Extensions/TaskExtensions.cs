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
    }
}
