﻿namespace GreatJorb.Business.Extensions
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
    }
}