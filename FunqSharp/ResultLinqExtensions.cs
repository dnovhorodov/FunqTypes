namespace FunqSharp;

public static class ResultLinqExtensions
{
    /// <summary>
    /// Applies a filter to the result, keeping success if the predicate matches.
    /// </summary>
    /// <param name="result">The value of the successful operation.</param>
    /// <param name="predicate">The predicate</param>
    /// <param name="error">The value of the error.</param>
    /// <typeparam name="T">An instance of <see cref="T"/></typeparam>
    /// <typeparam name="E">An instance of <see cref="E"/></typeparam>
    /// <returns>A new <see cref="Result{T, E}"/> instance.</returns>
    public static Result<T, E> Where<T, E>(this Result<T, E> result, Func<T, bool> predicate, E error)
    {
        if (!result.IsSuccess) return result;
            return predicate(result.Value) ? result : Result<T, E>.Fail(error);
    }
    
    /// <summary>
    /// Applies a filter to the result, keeping success if the predicate matches.
    /// </summary>
    /// <param name="result">The value of the successful operation.</param>
    /// <param name="predicate">The predicate</param>
    /// <typeparam name="T">An instance of <see cref="T"/></typeparam>
    /// <typeparam name="E">An instance of <see cref="E"/></typeparam>
    /// <returns></returns>
    public static Result<T, E> Where<T, E>(this Result<T, E> result, Func<T, bool> predicate)
    {
        return result.IsSuccess && predicate(result.Value) ? result : Result<T, E>.Fail();
    }
    
    /// <summary>
    /// Maps a successful result to a new type, keeping failures unchanged. 
    /// </summary>
    /// <param name="result">The value of the successful operation.</param>
    /// <param name="selector">The function that transforms <see cref="T"/> to <see cref="U"/></param>
    /// <typeparam name="T">The type of the current value.</typeparam>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <typeparam name="E">The type of error.</typeparam>
    /// <returns>A new <see cref="Result{U, E}"/> instance.</returns>
    public static Result<U, E> Select<T, U, E>(this Result<T, E> result, Func<T, U> selector)
    {
        return result.IsSuccess
            ? Result<U, E>.Ok(selector(result.Value))
            : Result<U, E>.Fail(result.Errors.ToArray());
    }

    /// <summary>
    /// Allows binding (flattening) inside LINQ queries.
    /// </summary>
    /// <param name="result">The value of the successful operation.</param>
    /// <param name="binder">The function that transforms <see cref="T"/> to <see cref="Result{U, E}"/></param>
    /// <typeparam name="T">The type of the current value.</typeparam>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <typeparam name="E">The type of error.</typeparam>
    /// <returns>A new <see cref="Result{U, E}"/> instance.</returns>
    public static Result<U, E> SelectMany<T, U, E>(this Result<T, E> result, Func<T, Result<U, E>> binder)
    {
        return result.IsSuccess ? binder(result.Value) : Result<U, E>.Fail(result.Errors.ToArray());
    }
}
