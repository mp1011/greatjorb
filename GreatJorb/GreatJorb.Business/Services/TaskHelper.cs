namespace GreatJorb.Business.Services;

public static class TaskHelper
{
    public static async Task<T?> RepeatUntilCondition<T>(Func<Task<T>> createTask, Predicate<T> condition, int maxTries=5)
        where T : class
    {
        int tries = 0;
        while(tries++ < maxTries)
        {
            var result = await createTask();
            if(condition(result))
                return result;

            await Task.Delay(100);
        }

        return null;
    }
}
