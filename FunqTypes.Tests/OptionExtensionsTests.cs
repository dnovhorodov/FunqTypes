namespace FunqTypes.Tests;

public class OptionExtensionsTests
{
    [Fact]
    public void ToOption_NonNull_IsSome() => Assert.Equal("a", "a".ToOption().GetValueOrDefault());

    [Fact]
    public void ToOption_Null_IsNone() => Assert.True(((string?)null).ToOption().IsNone);

    [Fact]
    public void ToOption_NullableStruct()
    {
        Assert.Equal(5, ((int?)5).ToOption().GetValueOrDefault());
        Assert.True(((int?)null).ToOption().IsNone);
    }

    [Fact]
    public void FirstOrNone_SkipsNulls() =>
        Assert.Equal("b", new string?[] { null, "b", "c" }.FirstOrNone().GetValueOrDefault());

    [Fact]
    public void FirstOrNone_EmptyOrAllNull_IsNone()
    {
        Assert.True(Array.Empty<string>().FirstOrNone().IsNone);
        Assert.True(new string?[] { null }.FirstOrNone().IsNone);
    }
}
