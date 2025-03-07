namespace FunqSharp;

/// <summary>
/// Represents an optional value which can be Some(T) or None.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly record struct Option<T>
{
    private readonly T? _value;

    /// <summary>
    /// Indicates whether the option contains a value.
    /// </summary>
    public bool IsSome { get; }
    
    /// <summary>
    /// Indicates whether the option contains no value.
    /// </summary>
    public bool IsNone => !IsSome;

    private Option(T value)
    {
        _value = value;
        IsSome = true;
    }

    /// <summary>
    /// Creates an option containing a value.
    /// </summary>
    /// <param name="value">The value to wrap in an option.</param>
    /// <returns>An option containing the provided value.</returns>
    public static Option<T> Some(T value) => new(value);
    
    /// <summary>
    /// Represents an empty option (None).
    /// </summary>
    /// <returns>An empty option.</returns>
    public static Option<T> None() => new();
    
    /// <summary>
    /// Funky alias for <see cref="Some(T)"/>.
    /// </summary>
    public static Option<T> Yeah(T value) => Some(value);

    /// <summary>
    /// Funky alias for <see cref="None"/>.
    /// </summary>
    public static Option<T> Nah() => None();

    /// <summary>
    /// Returns the contained value or the default value if None.
    /// </summary>
    /// <returns>The contained value if Some, otherwise the default value of T.</returns>
    public T GetValueOrDefault() => IsSome ? _value! : default!;
    
    /// <summary>
    /// Returns the contained value or a specified fallback value if None.
    /// </summary>
    /// <param name="defaultValue">The fallback value to return if None.</param>
    /// <returns>The contained value if Some, otherwise the specified fallback value.</returns>
    public T GetValueOrDefault(T defaultValue) => IsSome ? _value! : defaultValue!;
    
    /// <summary>
    /// Returns the contained value or computes a fallback value if None.
    /// </summary>
    /// <param name="defaultFactory">A function that provides a fallback value if None.</param>
    /// <returns>The contained value if Some, otherwise the result of <paramref name="defaultFactory"/>.</returns>
    public T GetValueOrDefault(Func<T> defaultFactory) => IsSome ? _value! : defaultFactory();

    /// <summary>
    /// Transforms an <see cref="Option{T}"/> into an <see cref="Option{U}"/> by applying a function to its value.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">A function to apply to the contained value.</param>
    /// <returns>An option containing the transformed value if Some, otherwise None.</returns>
    public Option<U> Map<U>(Func<T, U> func) =>
        IsSome ? Option<U>.Some(func(_value!)) : Option<U>.None();
    
    /// <summary>
    /// Transforms an <see cref="Option{T}"/> into another <see cref="Option{U}"/> by applying a function that returns an option.
    /// </summary>
    /// <typeparam name="U">The type of the transformed value.</typeparam>
    /// <param name="func">A function that maps the contained value to another option.</param>
    /// <returns>The resulting option if Some, otherwise None.</returns>
    public Option<U> Bind<U>(Func<T, Option<U>> func) =>
        IsSome ? func(_value!) : Option<U>.None();

    /// <summary>
    /// Executes an action if the option contains a value.
    /// </summary>
    /// <param name="action">The action to execute if Some.</param>
    public void IfSome(Action<T> action)
    {
        if (IsSome) action(_value!);
    }
    
    /// <summary>
    /// Alias for <see cref="IfSome"/>
    /// </summary>
    /// <param name="action"></param>
    public void IfYeah(Action<T> action) => IfSome(action);
    
    /// <summary>
    /// Executes an action if the option is None.
    /// </summary>
    /// <param name="action">The action to execute if None.</param>
    public void IfNone(Action action)
    {
        if (IsNone) action();
    }
    
    /// <summary>
    /// Alias for <see cref="IfNone"/>
    /// </summary>
    /// <param name="action"></param>
    public void IfNah(Action action) => IfNone(action);
    
    /// <summary>
    /// Converts an <see cref="Option{T}"/> into a <see cref="Result{T, E}"/>, using None as a failure case.
    /// </summary>
    /// <typeparam name="E">The type of the error when None.</typeparam>
    /// <param name="error">The error to use if None.</param>
    /// <returns>A <see cref="Result{T, E}"/> containing the value if Some, otherwise a failure with the provided error.</returns>
    public Result<T, E> ToResult<E>(E error) =>
        IsSome ? Result<T, E>.Ok(_value!) : Result<T, E>.Nah(error);
    
    /// <summary>
    /// Converts the Option to an enumerable sequence.
    /// </summary>
    /// <returns>An enumerable containing the value if Some, otherwise an empty sequence.</returns>
    public IEnumerable<T> ToEnumerable()
    {
        if (IsSome) yield return _value!;
    }
    
    /// <summary>
    /// Filters the Option based on a predicate. If the predicate fails, returns None.
    /// </summary>
    /// <param name="predicate">A function to test the contained value.</param>
    /// <returns>The original option if the predicate is true, otherwise None.</returns>
    public Option<T> Where(Func<T, bool> predicate) =>
        IsSome && predicate(_value!) ? this : None();

    /// <summary>
    /// Implicitly converts a value to an <see cref="Option{T}"/>.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Option<T>(T value) => value is not null ? Some(value) : None();
    
    /// <summary>
    /// Implicitly converts a <see cref="NoneType"/> to an <see cref="Option{T}"/>.
    /// </summary>
    /// <param name="_">A None value.</param>
    public static implicit operator Option<T>(NoneType _) => None();
}

/// <summary>
/// Represents the None value explicitly.
/// </summary>
public readonly struct NoneType { }

/// <summary>
/// Provides a None value for functional usage.
/// </summary>
public static class Option
{
    /// <summary>
    /// Represents an empty option (None).
    /// </summary>
    public static NoneType None => new();
}
