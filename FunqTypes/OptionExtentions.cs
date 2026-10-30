namespace FunqTypes;

public static class OptionExtentions
{
    public static Option<T> ToOption<T>(this T? value) where T : class =>
        value is null ? Option<T>.None() : Option<T>.Some(value);
    
    public static Option<T> ToOption<T>(this T? value) where T : struct =>
        value.HasValue ? Option<T>.Some(value.Value) : Option<T>.None();

    public static Option<T> FirstOrNone<T>(this IEnumerable<T?> source) where T : class =>
        source.FirstOrDefault(x => x is not null).ToOption();
}
