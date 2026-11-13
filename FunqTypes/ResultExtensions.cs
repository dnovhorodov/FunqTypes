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
    public static Result<TOut, TError> ToError<TIn, TOut, TError>(this Result<TIn, TError> result) =>
        result.IsSuccess
            ? throw new InvalidOperationException("Cannot convert success to error")
            : new(default!, false, result.Errors);
    
    /// <summary>
    /// Combines two successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2), E> Combine<T1, T2, E>(
        Result<T1, E> r1, Result<T2, E> r2)
    {
        if (r1.IsSuccess && r2.IsSuccess)
            return Result<(T1, T2), E>.Ok((r1.Value, r2.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines three successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3), E> Combine<T1, T2, T3, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess)
            return Result<(T1, T2, T3), E>.Ok((r1.Value, r2.Value, r3.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines four successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4), E> Combine<T1, T2, T3, T4, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess && r4.IsSuccess)
            return Result<(T1, T2, T3, T4), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines five successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5), E> Combine<T1, T2, T3, T4, T5, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess && r4.IsSuccess && r5.IsSuccess)
            return Result<(T1, T2, T3, T4, T5), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        if (!r5.IsSuccess) errors.AddRange(r5.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines six successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5, T6), E> Combine<T1, T2, T3, T4, T5, T6, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess && r4.IsSuccess && r5.IsSuccess && r6.IsSuccess)
            return Result<(T1, T2, T3, T4, T5, T6), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value, r6.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        if (!r5.IsSuccess) errors.AddRange(r5.Errors);
        if (!r6.IsSuccess) errors.AddRange(r6.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines seven successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5, T6, T7), E> Combine<T1, T2, T3, T4, T5, T6, T7, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6, Result<T7, E> r7)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess && r4.IsSuccess && r5.IsSuccess && r6.IsSuccess && r7.IsSuccess)
            return Result<(T1, T2, T3, T4, T5, T6, T7), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value, r6.Value, r7.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        if (!r5.IsSuccess) errors.AddRange(r5.Errors);
        if (!r6.IsSuccess) errors.AddRange(r6.Errors);
        if (!r7.IsSuccess) errors.AddRange(r7.Errors);
        return new(default!, false, errors);
    }

    /// <summary>
    /// Combines eight successful results into one tuple result. If any result failed, combines all errors.
    /// </summary>
    public static Result<(T1, T2, T3, T4, T5, T6, T7, T8), E> Combine<T1, T2, T3, T4, T5, T6, T7, T8, E>(
        Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6, Result<T7, E> r7, Result<T8, E> r8)
    {
        if (r1.IsSuccess && r2.IsSuccess && r3.IsSuccess && r4.IsSuccess && r5.IsSuccess && r6.IsSuccess && r7.IsSuccess && r8.IsSuccess)
            return Result<(T1, T2, T3, T4, T5, T6, T7, T8), E>.Ok((r1.Value, r2.Value, r3.Value, r4.Value, r5.Value, r6.Value, r7.Value, r8.Value));

        var errors = new List<E>();
        if (!r1.IsSuccess) errors.AddRange(r1.Errors);
        if (!r2.IsSuccess) errors.AddRange(r2.Errors);
        if (!r3.IsSuccess) errors.AddRange(r3.Errors);
        if (!r4.IsSuccess) errors.AddRange(r4.Errors);
        if (!r5.IsSuccess) errors.AddRange(r5.Errors);
        if (!r6.IsSuccess) errors.AddRange(r6.Errors);
        if (!r7.IsSuccess) errors.AddRange(r7.Errors);
        if (!r8.IsSuccess) errors.AddRange(r8.Errors);
        return new(default!, false, errors);
    }

    public static Result<U, E> Map<T1, T2, U, E>(this Result<(T1, T2), E> r, Func<T1, T2, U> f) =>
        r.Map(t => f(t.Item1, t.Item2));

    public static Result<U, E> Map<T1, T2, T3, U, E>(this Result<(T1, T2, T3), E> r, Func<T1, T2, T3, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3));

    public static Result<U, E> Map<T1, T2, T3, T4, U, E>(this Result<(T1, T2, T3, T4), E> r, Func<T1, T2, T3, T4, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3, t.Item4));

    public static Result<U, E> Map<T1, T2, T3, T4, T5, U, E>(this Result<(T1, T2, T3, T4, T5), E> r, Func<T1, T2, T3, T4, T5, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5));

    public static Result<U, E> Map<T1, T2, T3, T4, T5, T6, U, E>(this Result<(T1, T2, T3, T4, T5, T6), E> r, Func<T1, T2, T3, T4, T5, T6, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6));

    public static Result<U, E> Map<T1, T2, T3, T4, T5, T6, T7, U, E>(this Result<(T1, T2, T3, T4, T5, T6, T7), E> r, Func<T1, T2, T3, T4, T5, T6, T7, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7));

    public static Result<U, E> Map<T1, T2, T3, T4, T5, T6, T7, T8, U, E>(this Result<(T1, T2, T3, T4, T5, T6, T7, T8), E> r, Func<T1, T2, T3, T4, T5, T6, T7, T8, U> f) =>
        r.Map(t => f(t.Item1, t.Item2, t.Item3, t.Item4, t.Item5, t.Item6, t.Item7, t.Item8));
}

// capped at 8-tuple to match BCL ValueTuple direct arity; nest tuples if >8 needed
public static class Result
{
    public static Result<T, E> Combine<T, E>(params Result<T, E>[] results) =>
        Result<T, E>.Combine(results);

    public static Result<(T1, T2), E> Combine<T1, T2, E>(Result<T1, E> r1, Result<T2, E> r2) =>
        ResultExtensions.Combine(r1, r2);

    public static Result<(T1, T2, T3), E> Combine<T1, T2, T3, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3) =>
        ResultExtensions.Combine(r1, r2, r3);

    public static Result<(T1, T2, T3, T4), E> Combine<T1, T2, T3, T4, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4) =>
        ResultExtensions.Combine(r1, r2, r3, r4);

    public static Result<(T1, T2, T3, T4, T5), E> Combine<T1, T2, T3, T4, T5, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5) =>
        ResultExtensions.Combine(r1, r2, r3, r4, r5);

    public static Result<(T1, T2, T3, T4, T5, T6), E> Combine<T1, T2, T3, T4, T5, T6, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6) =>
        ResultExtensions.Combine(r1, r2, r3, r4, r5, r6);

    public static Result<(T1, T2, T3, T4, T5, T6, T7), E> Combine<T1, T2, T3, T4, T5, T6, T7, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6, Result<T7, E> r7) =>
        ResultExtensions.Combine(r1, r2, r3, r4, r5, r6, r7);

    public static Result<(T1, T2, T3, T4, T5, T6, T7, T8), E> Combine<T1, T2, T3, T4, T5, T6, T7, T8, E>(Result<T1, E> r1, Result<T2, E> r2, Result<T3, E> r3, Result<T4, E> r4, Result<T5, E> r5, Result<T6, E> r6, Result<T7, E> r7, Result<T8, E> r8) =>
        ResultExtensions.Combine(r1, r2, r3, r4, r5, r6, r7, r8);
}
