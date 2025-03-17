namespace FunqTypes;

public static class ResultAsyncExtensions
{
    /// <summary>
    /// Executes an asynchronous action on the successful value and returns the original result.
    /// Useful for async logging, debugging, or side effects without modifying the result. 
    /// </summary>
    /// <param name="resultTask">Async result</param>
    /// <param name="action">Async action on success value</param>
    /// <typeparam name="T">The value of the successful operation.</typeparam>
    /// <typeparam name="E">One or more error explaining the failure.</typeparam>
    /// <returns>Original result</returns>
    public static async Task<Result<T, E>> TapAsync<T, E>(this Task<Result<T, E>> resultTask, Func<T, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        if (result.IsSuccess)
            await action(result.Value).ConfigureAwait(false);
        
        return result;
    }
    
    /// <summary>
    /// Executes an asynchronous action on the error if the result is a failure.
    /// Useful for async logging, debugging, or side effects on failures without modifying the error.
    /// </summary>
    /// <param name="resultTask">Async result</param>
    /// <param name="action">Async action on errors</param>
    /// <typeparam name="T">The value of the successful operation.</typeparam>
    /// <typeparam name="E">One or more error explaining the failure.</typeparam>
    /// <returns>Original result</returns>
    public static async Task<Result<T, E>> TapErrorAsync<T, E>(this Task<Result<T, E>> resultTask, Func<List<E>, Task> action)
    {
        var result = await resultTask.ConfigureAwait(false);
        if (!result.IsSuccess)
            await action(result.Errors).ConfigureAwait(false);
        
        return result;
    }
}
