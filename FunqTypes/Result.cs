﻿namespace FunqTypes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Represents the outcome of an operation, encapsulating a success value or a collection of errors.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
/// <typeparam name="E">The type of the error</typeparam>
public readonly record struct Result<T, E>(T Value, bool IsSuccess, List<E> Errors)
{
    /// <summary>
    /// Creates a successful result with the provided value.
    /// </summary>
    /// <param name="value">The value of the successful operation.</param>
    /// <returns>A successful <see cref="Result{T, E}"/> instance.</returns>
    public static Result<T, E> Ok(T value) => new(value, true, new List<E>());

    /// <summary>
    /// Funky way to create a successful result.
    /// </summary>
    /// <param name="value">The value of the successful operation.</param>
    /// <returns>A successful <see cref="Result{T, E}"/> instance.</returns>
    public static Result<T, E> Gotcha(T value) => Ok(value);

    /// <summary>
    /// Creates a failed result with the provided error.
    /// </summary>
    /// <param name="errors">One or more error explaining the failure.</param>
    /// <returns>A failed <see cref="Result{T, E}"/> instance.</returns>
    public static Result<T, E> Fail(params E[] errors) => new(default!, false, errors.ToList());
    
    /// <summary>
    /// Funky way to create a failed result.
    /// </summary>
    /// <param name="errors">One or more error explaining the failure.</param>
    /// <returns>A failed <see cref="Result{T, E}"/> instance.</returns>
    public static Result<T, E> Oops(params E[] errors) => Fail(errors);

    /// <summary>
    /// Did this succeed?
    /// </summary>
    public bool IsGucci => IsSuccess;
    
    /// <summary>
    /// Returns the value if the result is a success; otherwise, returns the default value of <see cref="T"/>
    /// </summary>
    /// <returns>An instance of <see cref="T"/></returns>
    public T GetValueOrDefault() => IsSuccess ? Value : default!;
    
    /// <summary>
    /// Returns the value if the result is a success; otherwise, returns the specified default value
    /// </summary>
    /// <returns>Default value</returns>
    public T GetValueOrDefault(T defaultValue) => IsSuccess ? Value : defaultValue;
    
    /// <summary>
    /// Returns the value if the result is a success; otherwise, returns a value provided by a factory function
    /// </summary>
    /// <returns>Default value</returns>
    public T GetValueOrDefault(Func<T> defaultValueFactory) => IsSuccess ? Value : defaultValueFactory();

    /// <summary>
    /// Transforms the value of a successful result using the provided function.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">A function to transform the success value.</param>
    /// <returns>A new <see cref="Result{U, E}"/> instance with the transformed value or the existing errors.</returns>
    public Result<U, E> Map<U>(Func<T, U> func) =>
        IsSuccess ?
            Result<U, E>.Ok(func(Value))
            : Result<U, E>.Fail(Errors.ToArray());
    
    /// <summary>
    /// Transforms the value of a failed result using provided function
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="F">The type of the transformed value.</typeparam>
    /// <returns>A new <see cref="Result{T, F}"/> instance with the transformed errors or the existing value.</returns>
    public Result<T, F> MapError<F>(Func<E, F> func) =>
        IsSuccess
            ? Result<T, F>.Ok(Value)
            : Result<T, F>.Fail(Errors.Select(func).ToArray());

    /// <summary>
    /// Asynchronously transforms the value of a successful result using the provided function.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">An asynchronous function to transform the success value.</param>
    /// <returns>A task representing a new <see cref="Result{U, E}"/> instance.</returns>
    public async Task<Result<U, E>> MapAsync<U>(Func<T, Task<U>> func) =>
        IsSuccess
            ? Result<U, E>.Ok(await func(Value).ConfigureAwait(false))
            : Result<U, E>.Fail(Errors.ToArray());

    /// <summary>
    /// Chains another result-producing function, propagating errors if the current result is a failure.
    /// </summary>
    /// <typeparam name="U">The type of the new result.</typeparam>
    /// <param name="func">A function that takes the success value and returns a new <see cref="Result{U, E}"/>.</param>
    /// <returns>A new <see cref="Result{U, E}"/> instance.</returns>
    public Result<U, E> Bind<U>(Func<T, Result<U, E>> func) =>
        IsSuccess ? func(Value) : Result<U, E>.Fail(Errors.ToArray());

    /// <summary>
    /// Asynchronously chains another result-producing function.
    /// </summary>
    /// <typeparam name="U">The type of the new result.</typeparam>
    /// <param name="func">An asynchronous function that takes the success value and returns a new <see cref="Result{U, E}"/>.</param>
    /// <returns>A task representing a new <see cref="Result{U, E}"/> instance.</returns>
    public async Task<Result<U, E>> BindAsync<U>(Func<T, Task<Result<U, E>>> func) =>
        IsSuccess ?
            await func(Value).ConfigureAwait(false)
            : Result<U, E>.Fail(Errors.ToArray());

    /// <summary>
    /// Combines multiple results, accumulating errors if any exist.
    /// </summary>
    /// <param name="results">An array of results to combine.</param>
    /// <returns>A single <see cref="Result{T, E}"/> containing the first valid value or all accumulated errors.</returns>
    public static Result<T, E> Combine(params Result<T, E>[] results)
    {
        var errors = results.SelectMany(r => r.Errors).ToList();
        return errors.Count != 0 ? Fail(errors.ToArray()) : Ok(results[0].Value);
    }

    /// <summary>
    /// Performs pattern matching on the result.
    /// </summary>
    /// <typeparam name="U">The return type of the match functions.</typeparam>
    /// <param name="onSuccess">Function executed if the result is successful.</param>
    /// <param name="onFailure">Function executed if the result is a failure.</param>
    /// <returns>The result of either function.</returns>
    public U Match<U>(Func<T, U> onSuccess, Func<List<E>, U> onFailure) =>
        IsSuccess ? onSuccess(Value) : onFailure(Errors);

    /// <summary>
    /// Ensures the success value meets a condition; otherwise, returns a failure.
    /// </summary>
    /// <param name="predicate">A function that validates the success value.</param>
    /// <param name="error">An error to return if the validation fails.</param>
    /// <returns>A new <see cref="Result{T, E}"/> based on the validation.</returns>
    public Result<T, E> Ensure(Func<T, bool> predicate, E error) =>
        IsSuccess && predicate(Value) ? this : Fail(error);
    
    /// <summary>
    /// Asynchronously ensures the success or returns a failure.
    /// </summary>
    /// <param name="predicate">A function that validates the success value.</param>
    /// <param name="error">An error to return if the validation fails.</param>
    /// <returns>A new <see cref="Result{T, E}"/> based on the validation.</returns>
    public async Task<Result<T, E>> EnsureAsync(Func<T, Task<bool>> predicate, E error)
    {
        return IsSuccess && !await predicate(Value).ConfigureAwait(false)
            ? Fail(error)
            : this;
    }

    /// <summary>
    /// Executes an action on the successful value and returns the original result.
    /// Useful for logging, debugging, or running side effects without modifying the result.
    /// </summary>
    /// <param name="action">Action on success value</param>
    /// <returns>The same instance of <see cref="Result{T, E}"/></returns>
    public Result<T, E> Tap(Action<T> action)
    {
        if(IsSuccess)
            action(Value);

        return this;
    }

    /// <summary>
    /// Executes an action on the error if the result is a failure.
    /// Useful for logging, debugging, or running side effects without modifying the result.
    /// </summary>
    /// <param name="action">Action on the list of errors</param>
    /// <returns>The same instance of <see cref="Result{T, E}"/></returns>
    public Result<T, E> TapError(Action<List<E>> action)
    {
        if (!IsSuccess)
            action(Errors);

        return this;
    }
    
    /// <summary>
    /// Implicitly converts a value into a successful result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<T, E>(T value) => Ok(value);
}
