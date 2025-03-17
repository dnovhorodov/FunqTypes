namespace FunqTypes;

public static class ResultAsyncExtensions
{
    /// <summary>
    /// Asynchronously transforms the value of a successful async result using the provided function.
    /// </summary>
    /// <param name="resultTask">Async result</param>
    /// <param name="mapper">An asynchronous function to transform the success value.</param>
    /// <typeparam name="T">The current value type.</typeparam>
    /// <typeparam name="U">The new value type after transformation.</typeparam>
    /// <typeparam name="E">The error type.</typeparam>
    /// <returns>A new <see cref="Result{U, E}"/> with transformed value if success, or the original error.</returns>
    public static async Task<Result<U, E>> MapAsync<T, U, E>(
        this Task<Result<T, E>> resultTask,
        Func<T, Task<U>> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? Result<U, E>.Ok(await mapper(result.Value).ConfigureAwait(false))
            : Result<U, E>.Fail(result.Errors.ToArray());
    }
    
    /// <summary>
    /// Asynchronously transforms the error type of a failed <see cref="Result{T, E}"/>.
    /// Leaves the success value unchanged.
    /// </summary>
    /// <typeparam name="T">The success type.</typeparam>
    /// <typeparam name="E">The current error type.</typeparam>
    /// <typeparam name="F">The new error type after transformation.</typeparam>
    /// <param name="resultTask">The asynchronous result to transform.</param>
    /// <param name="errorMapper">The asynchronous function to map existing errors to a new type.</param>
    /// <returns>A new <see cref="Result{T, F}"/> with transformed errors if failure, or the original success.</returns>
    public static async Task<Result<T, F>> MapErrorAsync<T, E, F>(
        this Task<Result<T, E>> resultTask, Func<E, Task<F>> errorMapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? Result<T, F>.Ok(result.Value)
            : Result<T, F>.Fail(await Task.WhenAll(result.Errors.Select(errorMapper)).ConfigureAwait(false));
    }
    
    /// <summary>
    /// Asynchronously chains another async result-producing function.
    /// </summary>
    /// <param name="resultTask">Async result</param>
    /// <param name="binder">An asynchronous function that takes the success value and returns a new <see cref="Result{U, E}"/>.</param>
    /// <typeparam name="T">The type of the current result.</typeparam>
    /// <typeparam name="U">The type of the new result.</typeparam>
    /// <typeparam name="E">The type of the error.</typeparam>
    /// <returns>A task representing a new <see cref="Result{U, E}"/> instance.</returns>
    public static async Task<Result<U, E>> BindAsync<T, U, E>(
        this Task<Result<T, E>> resultTask,
        Func<T, Task<Result<U, E>>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await binder(result.Value).ConfigureAwait(false)
            : Result<U, E>.Fail(result.Errors.ToArray());
    }
    
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
