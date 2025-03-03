namespace FunqSharp.Tests;

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
    public void Yeah_ShouldBehaveExactlyLike_Ok()
    {
        const string Value = "FunqSharp Rocks!";

        var yeahResult = Result<string, FunqError>.Yeah(Value);
        var okResult = Result<string, FunqError>.Ok(Value);

        Assert.True(yeahResult.IsSuccess, "Yeah should be successful.");
        Assert.True(okResult.IsSuccess, "Success should be successful.");
        Assert.Equivalent(okResult, yeahResult);
    }

    [Fact]
    public void Nope_ShouldBehaveExactlyLike_Fail()
    {
        var error = new FunqError("FAIL", "Test-driven sadness");

        var nopeResult = Nope(error);
        var failureResult = Fail(error);

        Assert.False(nopeResult.IsSuccess, "Nope should be a failure.");
        Assert.False(failureResult.IsSuccess, "Failure should be a failure.");
        Assert.Equivalent(failureResult, nopeResult);
    }
    
    [Fact]
    public void IsNeat_ShouldBeTrueFor_Yeah()
    {
        var result = Yeah(42);

        Assert.True(result.IsNeat, "IsYeah should return true for a successful result.");
    }

    [Fact]
    public void IsNeat_ShouldBeFalseFor_Nope()
    {
        var result = Nope(new FunqError("NOPE", "Not today!"));

        Assert.False(result.IsNeat, "IsYeah should return false for a failure result.");
    }

    [Fact]
    public void IsNeat_ShouldBeAliasFor_IsSuccess()
    {
        var yeahResult = Yeah(100);
        var nopeResult = Nope(new FunqError("FAIL", "Nope'd"));

        Assert.Equal(yeahResult.IsSuccess, yeahResult.IsNeat);
        Assert.Equal(nopeResult.IsSuccess, nopeResult.IsNeat);
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
        var initial = Yeah(99);

        var mutated = initial with { Value = 100 };

        Assert.NotEqual(initial, mutated);
    }

    #endregion
}
