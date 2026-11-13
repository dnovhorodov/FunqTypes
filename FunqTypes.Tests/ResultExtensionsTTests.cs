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

    [Fact]
    public void Combine2_To_8_And_DeconstructingMap_ShouldWork()
    {
        var r2 = Result.Combine(Result<int, string>.Ok(1), Result<string, string>.Ok("2"))
            .Map((a, b) => $"{a}-{b}");
        Assert.Equal("1-2", r2.Value);

        var r3 = Result.Combine(Result<int, string>.Ok(1), Result<string, string>.Ok("2"), Result<int, string>.Ok(3))
            .Map((a, b, c) => $"{a}-{b}-{c}");
        Assert.Equal("1-2-3", r3.Value);

        var r4 = Result.Combine(Result<int, string>.Ok(1), Result<int, string>.Ok(2), Result<int, string>.Ok(3), Result<int, string>.Ok(4))
            .Map((a, b, c, d) => a + b + c + d);
        Assert.Equal(10, r4.Value);

        var r5 = Result.Combine(Result<int, string>.Ok(1), Result<int, string>.Ok(2), Result<int, string>.Ok(3), Result<int, string>.Ok(4), Result<int, string>.Ok(5))
            .Map((a, b, c, d, e) => a + b + c + d + e);
        Assert.Equal(15, r5.Value);

        var r6 = Result.Combine(
            Result<int, string>.Ok(1),
            Result<string, string>.Ok("2"),
            Result<double, string>.Ok(3.0),
            Result<bool, string>.Ok(true),
            Result<char, string>.Ok('5'),
            Result<long, string>.Ok(6L)
        ).Map((a, b, c, d, e, f) => $"{a}-{b}-{c}-{d}-{e}-{f}");
        Assert.True(r6.IsSuccess);
        Assert.Equal("1-2-3-True-5-6", r6.Value);

        var r7 = Result.Combine(
            Result<int, string>.Ok(1),
            Result<int, string>.Ok(2),
            Result<int, string>.Ok(3),
            Result<int, string>.Ok(4),
            Result<int, string>.Ok(5),
            Result<int, string>.Ok(6),
            Result<int, string>.Ok(7)
        ).Map((a, b, c, d, e, f, g) => a + b + c + d + e + f + g);
        Assert.Equal(28, r7.Value);

        var r8Success = Result.Combine(
            Result<int, string>.Ok(1),
            Result<int, string>.Ok(2),
            Result<int, string>.Ok(3),
            Result<int, string>.Ok(4),
            Result<int, string>.Ok(5),
            Result<int, string>.Ok(6),
            Result<int, string>.Ok(7),
            Result<string, string>.Ok("8")
        ).Map((a, b, c, d, e, f, g, h) => $"{a + b + c + d + e + f + g}:{h}");
        Assert.Equal("28:8", r8Success.Value);

        var r8Fail = Result.Combine(
            Result<int, string>.Ok(1),
            Result<int, string>.Fail("e2"),
            Result<int, string>.Ok(3),
            Result<int, string>.Ok(4),
            Result<int, string>.Ok(5),
            Result<int, string>.Fail("e6"),
            Result<int, string>.Ok(7),
            Result<string, string>.Fail("e8")
        ).Map((a, b, c, d, e, f, g, h) => h);
        Assert.False(r8Fail.IsSuccess);
        Assert.Equal(new[] { "e2", "e6", "e8" }, r8Fail.Errors);
    }
}
