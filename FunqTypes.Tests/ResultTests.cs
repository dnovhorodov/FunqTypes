namespace FunqTypes.Tests;

public record FunqError(string Code, string Message);

public class ResultTests
{
    #region Success and Failure Tests

    [Fact]
    public void Ok_ShouldCreateSuccessResult()
    {
        const int Value = 42;
        var result = Ok(Value);

        Assert.True(result.IsSuccess);
        Assert.Equal(Value, result.Value);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Fail_ShouldCreateFailureResult()
    {
        var error = new FunqError("ERROR", "Something went wrong");
        var result = Fail(error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    [Fact]
    public void Gotcha_ShouldBehaveExactlyLike_Ok()
    {
        const string Value = "FunqTypes Rocks!";

        var gotchaResult = Result<string, FunqError>.Gotcha(Value);
        var okResult = Result<string, FunqError>.Ok(Value);

        Assert.True(gotchaResult.IsSuccess, "gotcha should be successful.");
        Assert.True(okResult.IsSuccess, "Success should be successful.");
        Assert.Equivalent(okResult, gotchaResult);
    }

    [Fact]
    public void Oops_ShouldBehaveExactlyLike_Fail()
    {
        var error = new FunqError("FAIL", "Test-driven sadness");

        var oopsResult = Oops(error);
        var failureResult = Fail(error);

        Assert.False(oopsResult.IsSuccess, "Oops should be a failure.");
        Assert.False(failureResult.IsSuccess, "Failure should be a failure.");
        Assert.Equivalent(failureResult, oopsResult);
    }

    [Fact]
    public void IsGucci_ShouldBeTrueFor_Gotcha()
    {
        var result = Result<int, FunqError>.Gotcha(42);

        Assert.True(result.IsGucci, "IsGucci should return true for a successful result.");
    }

    [Fact]
    public void IsGucci_ShouldBeFalseFor_Oops()
    {
        var result = Oops(new FunqError("Oops", "Not today!"));

        Assert.False(result.IsGucci, "IsGucci should return false for a failure result.");
    }

    [Fact]
    public void IsGucci_ShouldBeAliasFor_IsSuccess()
    {
        var gotchaResult = Result<int, FunqError>.Gotcha(100);
        var oopsResult = Oops(new FunqError("FAIL", "Oops'd"));

        Assert.Equal(gotchaResult.IsSuccess, gotchaResult.IsGucci);
        Assert.Equal(oopsResult.IsSuccess, oopsResult.IsGucci);
    }
    
    #endregion

    #region GetDefaultValue tests
    
    [Fact]
    public void GetValueOrDefault_ShouldReturnSuccessValue_WhenResultIsSuccess()
    {
        var result = Result<int, string>.Ok(42);

        var value = result.GetValueOrDefault();

        Assert.Equal(42, value);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnDefaultValue_WhenResultIsFailure()
    {
        var result = Result<int, string>.Fail("Error!");

        var value = result.GetValueOrDefault();

        Assert.Equal(0, value);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnProvidedDefaultValue_WhenResultIsFailure()
    {
        var result = Result<string, string>.Fail("Error!");

        var value = result.GetValueOrDefault("Fallback Value");

        Assert.Equal("Fallback Value", value);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnSuccessValue_WhenResultIsSuccess_AndCustomDefaultIsProvided()
    {
        var result = Result<string, string>.Ok("Success!");

        var value = result.GetValueOrDefault("Fallback Value");

        Assert.Equal("Success!", value);
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnFactoryValue_WhenResultIsFailure()
    {
        var result = Result<DateTime, string>.Oops("Error!");

        var value = result.GetValueOrDefault(() => new DateTime(2024, 1, 1));

        Assert.Equal(new DateTime(2024, 1, 1), value);
    }

    [Fact]
    public void GetValueOrDefault_ShouldNotCallFactory_WhenResultIsSuccess()
    {
        var result = Result<int, string>.Ok(100);
        var factoryCalled = false;

        var value = result.GetValueOrDefault(() =>
        {
            factoryCalled = true;
            return -1;
        });

        Assert.Equal(100, value);
        Assert.False(factoryCalled, "Factory should not be called for success results.");
    }

    #endregion

    #region Bind Tests

    [Fact]
    public void Bind_ShouldTransformSuccessResult()
    {
        var result = Ok(5)
            .Bind(x => Ok(x * 2));

        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public void Bind_ShouldPropagateFailure()
    {
        var error = new FunqError("INVALID", "Invalid input");
        var result = Fail(error)
            .Bind(x => Ok(x * 2));

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    #endregion

    #region Map Tests

    [Fact]
    public void Map_ShouldTransformSuccessResult()
    {
        var result = Ok(10)
            .Map(x => x * 3);

        Assert.True(result.IsSuccess);
        Assert.Equal(30, result.Value);
    }

    [Fact]
    public void Map_ShouldPropagateFailure()
    {
        var error = new FunqError("FAIL", "Mapping failed");
        var result = Fail(error)
            .Map(x => x * 2);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    #endregion

    #region Ensure Tests

    [Fact]
    public void Ensure_ShouldPassWhenConditionIsMet()
    {
        var result = Ok(25)
            .Ensure(x => x >= 18, new FunqError("AGE_INVALID", "Age must be 18 or older"));

        Assert.True(result.IsSuccess);
        Assert.Equal(25, result.Value);
    }

    [Fact]
    public void Ensure_ShouldFailWhenConditionIsNotMet()
    {
        var error = new FunqError("AGE_INVALID", "Age must be 18 or older");
        var result = Ok(16)
            .Ensure(x => x >= 18, error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    #endregion

    #region Combine Tests

    [Fact]
    public void Combine_ShouldReturnFirstSuccessResultIfAllAreValid()
    {
        var result = Combine(
            Ok(1),
            Ok(2),
            Ok(3)
        );

        Assert.True(result.IsSuccess);
        Assert.Equal(1, result.Value);
    }

    [Fact]
    public void Combine_ShouldAccumulateErrorsIfAnyFail()
    {
        var error1 = new FunqError("ERR1", "First error");
        var error2 = new FunqError("ERR2", "Second error");

        var result = Combine(
            Ok(1),
            Fail(error1),
            Fail(error2)
        );

        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(error1, result.Errors);
        Assert.Contains(error2, result.Errors);
    }

    #endregion

    #region Match Tests

    [Fact]
    public void Match_ShouldExecuteOnSuccessFunction()
    {
        var result = Ok(100);

        var output = result.Match(
            success => $"Success: {success}",
            errors => $"Errors: {string.Join(", ", errors.Select(e => e.Message))}"
        );

        Assert.Equal("Success: 100", output);
    }

    [Fact]
    public void Match_ShouldExecuteOnFailureFunction()
    {
        var error = new FunqError("ERROR", "Something went wrong");
        var result = Fail(error);

        var output = result.Match(
            success => $"Success: {success}",
            errors => $"Errors: {string.Join(", ", errors.Select(e => e.Message))}"
        );

        Assert.Equal("Errors: Something went wrong", output);
    }

    #endregion
    
    #region Tap() (Sync)
    
    [Fact]
    public void Tap_ShouldExecuteAction_WhenResultIsSuccess()
    {
        var result = Result<int, string>.Gotcha(42);
        var capturedValue = 0;

        result = result.Tap(value => capturedValue = value);

        Assert.True(result.IsGucci);
        Assert.Equal(42, result.Value);
        Assert.Equal(42, capturedValue);
    }

    [Fact]
    public void Tap_ShouldNotExecuteAction_WhenResultIsFailure()
    {
        var result = Result<int, string>.Oops("Error occurred");
        var capturedValue = 0;

        result = result.Tap(value => capturedValue = value);

        Assert.False(result.IsGucci);
        Assert.NotEmpty(result.Errors);
        Assert.Equal(0, capturedValue);
    }

    #endregion
    
    #region TapError() (Sync)

    [Fact]
    public void TapError_ShouldExecuteAction_WhenResultIsFailure()
    {
        var result = Result<int, string>.Oops("Something went wrong");
        List<string> capturedErrors = new();

        result = result.TapError(errors => capturedErrors.AddRange(errors));

        Assert.False(result.IsGucci);
        Assert.Single(result.Errors);
        Assert.Equal("Something went wrong", capturedErrors.First());
    }

    [Fact]
    public void TapError_ShouldNotExecuteAction_WhenResultIsSuccess()
    {
        var result = Result<int, string>.Gotcha(42);
        List<string> capturedErrors = new();

        result = result.TapError(errors => capturedErrors.AddRange(errors));

        Assert.True(result.IsGucci);
        Assert.Equal(42, result.Value);
        Assert.Empty(capturedErrors);
    }

    #endregion

    #region Async Tests

    [Fact]
    public async Task BindAsync_ShouldTransformSuccessResult()
    {
        var result = await Ok(10)
            .BindAsync(async x => await Task.FromResult(Ok(x * 2)));

        Assert.True(result.IsSuccess);
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public async Task BindAsync_ShouldPropagateFailure()
    {
        var error = new FunqError("ASYNC_ERROR", "Async operation failed");
        var result = await Fail(error)
            .BindAsync(async x => await Task.FromResult(Ok(x * 2)));

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    [Fact]
    public async Task EnsureAsync_ShouldValidateSuccessfully()
    {
        var result = await Ok(25)
            .EnsureAsync(async x => await Task.FromResult(x >= 18),
                new FunqError("AGE_FAIL", "Age must be 18 or older"));

        Assert.True(result.IsSuccess);
        Assert.Equal(25, result.Value);
    }

    [Fact]
    public async Task EnsureAsync_ShouldFailWhenConditionIsNotMet()
    {
        var error = new FunqError("AGE_FAIL", "Age must be 18 or older");
        var result = await Ok(16)
            .EnsureAsync(async x => await Task.FromResult(x >= 18), error);

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal(error, result.Errors.First());
    }

    [Fact]
    public void Ok_ShouldNotMutate()
    {
        var initial = Result<int, FunqError>.Gotcha(99);

        var mutated = initial with { Value = 100 };

        Assert.NotEqual(initial, mutated);
    }
    
    [Fact]
    public async Task TapAsync_ShouldExecuteAction_WhenResultIsSuccess()
    {
        var result = Task.FromResult(Result<int, string>.Gotcha(42));
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
        var result = Task.FromResult(Result<int, string>.Oops("Error"));
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
