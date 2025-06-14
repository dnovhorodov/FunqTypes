namespace FunqTypes;

public static class ResultExtensions
{
    /// <summary>
    /// Converts an error result of type <typeparamref name="TIn"/> to a result of type <typeparamref name="TOut"/>, retaining the original error(s).
    /// </summary>
    /// <typeparam name="TIn">The input type of the original result.</typeparam>
    /// <typeparam name="TOut">The output type of the resulting error result.</typeparam>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>A failed result of type <typeparamref name="TOut"/> containing the same errors.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the original result is a success.</exception>
    public static Result<TOut, TError> ToError<TIn, TOut, TError>(this Result<TIn, TError> result)
    {
        return result.Match<Result<TOut, TError>>(
            ok => throw new InvalidOperationException("Cannot convert success to error"),
            err => Result<TOut, TError>.Fail(err.ToArray())
        );
    }
    
    /// <summary>
    /// Combines two successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2), E> Combine<T1, T2, E>(
        Result<T1, E> r1, Result<T2, E> r2)
    {
        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);

        return errors.Count > 0
            ? Result<(T1, T2), E>.Fail(errors.ToArray())
            : Result<(T1, T2), E>.Ok((r1.Value, r2.Value));
    }

    /// <summary>
    /// Combines three successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3), E> Combine<T1, T2, T3, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3)
    {
        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);

        return errors.Count > 0
            ? Result<(T1, T2, T3), E>.Fail(errors.ToArray())
            : Result<(T1, T2, T3), E>.Ok((r1.Value, r2.Value, r3.Value));
    }

    /// <summary>
    /// Combines four successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4), E> Combine<T1, T2, T3, T4, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4)
    {
        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);

        return errors.Count > 0
            ? Result<(T1, T2, T3, T4), E>.Fail(errors.ToArray())
            : Result<(T1, T2, T3, T4), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value));
    }

    /// <summary>
    /// Combines five successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5), E> Combine<T1, T2, T3, T4, T5, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5)
    {
        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        if (!r5.IsSuccess) errors.AddRange(r5.Errors);

        return errors.Count > 0
            ? Result<(T1, T2, T3, T4, T5), E>.Fail(errors.ToArray())
            : Result<(T1, T2, T3, T4, T5), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value));
    }
}
