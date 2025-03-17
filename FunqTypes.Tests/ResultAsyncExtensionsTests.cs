namespace FunqTypes.Tests;

public class ResultAsyncExtensionsTests
{
    #region MapAsync() Tests

    [Fact]
    public async Task MapAsync_ShouldTransformValue_WhenSuccess()
    {
        var result = Task.FromResult(Result<int, string>.Ok(42));

        var mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(10);
            return x * 2;
        });

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(84, mappedResult.Value);
    }

    [Fact]
    public async Task MapAsync_ShouldNotTransform_WhenFailure()
    {
        var result = Task.FromResult(Result<int, string>.Fail("Original Error"));

        var mappedResult = await result.MapAsync(async x =>
        {
            await Task.Delay(10);
            return x * 2;
        });

        Assert.False(mappedResult.IsSuccess);
        Assert.Single(mappedResult.Errors);
        Assert.Equal("Original Error", mappedResult.Errors.First());
    }

    #endregion

    #region MapErrorAsync() Tests

    [Fact]
    public async Task MapErrorAsync_ShouldTransformError_WhenFailure()
    {
        var result = Task.FromResult(Result<int, string>.Fail("Critical Failure"));

        var mappedResult = await result.MapErrorAsync(async error =>
        {
            await Task.Delay(10);
            return new ErrorDetails(error, DateTime.UtcNow);
        });

        Assert.False(mappedResult.IsSuccess);
        Assert.Single(mappedResult.Errors);
        Assert.Contains("Critical Failure", mappedResult.Errors.First().Message);
    }

    [Fact]
    public async Task MapErrorAsync_ShouldNotTransform_WhenSuccess()
    {
        var result = Task.FromResult(Result<int, string>.Ok(42));

        var mappedResult = await result.MapErrorAsync(async error =>
        {
            await Task.Delay(10);
            return new ErrorDetails(error, DateTime.UtcNow);
        });

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(42, mappedResult.Value);
        Assert.Empty(mappedResult.Errors);
    }

    #endregion

    #region BindAsync() Tests

    [Fact]
    public async Task BindAsync_ShouldFlattenAndChainResults_WhenSuccess()
    {
        var result = Task.FromResult(Result<int, string>.Ok(5));

        async Task<Result<string, string>> AppendText(int value)
        {
            await Task.Delay(10);
            return Result<string, string>.Ok($"Processed-{value}");
        }

        var chainedResult = await result.BindAsync(AppendText);

        Assert.True(chainedResult.IsSuccess);
        Assert.Equal("Processed-5", chainedResult.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateFailure_WithoutCallingBinder()
    {
        var result = Task.FromResult(Result<int, string>.Fail("Failed Input"));

        async Task<Result<string, string>> AppendText(int value)
        {
            await Task.Delay(10);
            return Result<string, string>.Fail($"Processed-{value}");
        }

        var chainedResult = await result.BindAsync(AppendText);

        Assert.False(chainedResult.IsSuccess);
        Assert.Single(chainedResult.Errors);
        Assert.Equal("Failed Input", chainedResult.Errors.First());
    }

    #endregion

    #region TapAsync() Tests

    [Fact]
    public async Task TapAsync_ShouldExecuteAction_WhenResultIsSuccess()
    {
        var result = Task.FromResult(Result<int, string>.Ok(42));
        int capturedValue = 0;

        var finalResult = await result.TapAsync(async value =>
        {
            await Task.Delay(10);
            capturedValue = value;
        });

        Assert.True(finalResult.IsGucci);
        Assert.Equal(42, finalResult.Value);
        Assert.Equal(42, capturedValue);
    }

    [Fact]
    public async Task TapAsync_ShouldNotExecuteAction_WhenResultIsFailure()
    {
        var result = Task.FromResult(Result<int, string>.Fail("Error"));
        int capturedValue = 0;

        var finalResult = await result.TapAsync(async value =>
        {
            await Task.Delay(10);
            capturedValue = value;
        });

        Assert.False(finalResult.IsGucci);
        Assert.NotEmpty(finalResult.Errors);
        Assert.Equal(0, capturedValue);
    }

    #endregion
}
