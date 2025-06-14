using static FunqTypes.ResultExtensions;

namespace FunqTypes.Tests;

public class ResultExtensionsTTests
{
    [Fact]
    public void ToError_ShouldThrow_WhenResultIsSuccess()
    {
        var result = Result<int, string>.Ok(42);
        Assert.Throws<InvalidOperationException>(() => result.ToError<int, string, string>());
    }

    [Fact]
    public void ToError_ShouldReturnError_WhenResultIsFail()
    {
        var result = Result<int, string>.Fail("err");
        var converted = result.ToError<int, string, string>();
        Assert.False(converted.IsSuccess);
        Assert.Equal("err", converted.Errors.Single());
    }

    [Fact]
    public void Combine2_ShouldReturnOk_WhenAllSuccess()
    {
        var result = Combine(Result<int, string>.Ok(1), Result<string, string>.Ok("A"));
        Assert.True(result.IsSuccess);
        Assert.Equal((1, "A"), result.Value);
    }

    [Fact]
    public void Combine2_ShouldReturnErrors_WhenFailures()
    {
        var result = Combine(Result<int, string>.Fail("err1"), Result<string, string>.Fail("err2"));
        Assert.False(result.IsSuccess);
        Assert.Equal(new[] { "err1", "err2" }, result.Errors);
    }

    [Fact]
    public void Combine3_ShouldReturnOk_WhenAllSuccess()
    {
        var result = Combine(Result<int, string>.Ok(1), Result<string, string>.Ok("A"), Result<double, string>.Ok(3.0));
        Assert.True(result.IsSuccess);
        Assert.Equal((1, "A", 3.0), result.Value);
    }

    [Fact]
    public void Combine4_ShouldReturnOk_WhenAllSuccess()
    {
        var result = Combine(
            Result<int, string>.Ok(1),
            Result<string, string>.Ok("A"),
            Result<double, string>.Ok(3.0),
            Result<bool, string>.Ok(true));
        Assert.True(result.IsSuccess);
        Assert.Equal((1, "A", 3.0, true), result.Value);
    }

    [Fact]
    public void Combine5_ShouldReturnErrors_WhenSomeFail()
    {
        var result = ResultExtensions.Combine(
            Result<int, string>.Ok(1),
            Result<string, string>.Fail("e2"),
            Result<int, string>.Fail("e3"),
            Result<bool, string>.Ok(true),
            Result<double, string>.Fail("e5")
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(new[] { "e2", "e3", "e5" }, result.Errors);
    }
}
