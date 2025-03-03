namespace FunqSharp.Tests;

public class ResultLinqExtensionsTests
{
    #region Where Tests

    [Fact]
    public void Where_ShouldReturnSuccess_IfConditionIsMet()
    {
        var result = Ok(10);

        var filteredResult = result.Where(x => x > 5, new FunqError("TOO_SMALL", "Number must be greater than 5"));

        Assert.True(filteredResult.IsSuccess);
        Assert.Equal(10, filteredResult.Value);
    }

    [Fact]
    public void Where_ShouldReturnFailure_IfConditionIsNotMet()
    {
        var result = Ok(3);

        var filteredResult = result.Where(x => x > 5, new FunqError("TOO_SMALL", "Number must be greater than 5"));

        Assert.False(filteredResult.IsSuccess);
        Assert.Single(filteredResult.Errors);
        Assert.Equal("TOO_SMALL", filteredResult.Errors.First().Code);
    }

    [Fact]
    public void Where_ShouldNotAffectFailureResults()
    {
        var error = new FunqError("INVALID", "Initial failure");
        var result = Fail(error);

        var filteredResult = result.Where(x => x > 5, new FunqError("TOO_SMALL", "Number must be greater than 5"));

        Assert.False(filteredResult.IsSuccess);
        Assert.Single(filteredResult.Errors);
        Assert.Equal(error, filteredResult.Errors.First());
    }

    #endregion

    #region Select Tests

    [Fact]
    public void Select_ShouldTransformSuccessValue()
    {
        var result = Ok(5);

        var mappedResult = result.Select(x => x * 2);

        Assert.True(mappedResult.IsSuccess);
        Assert.Equal(10, mappedResult.Value);
    }

    [Fact]
    public void Select_ShouldPreserveErrors()
    {
        var error = new FunqError("INVALID", "Cannot process");
        var result = Fail(error);

        var mappedResult = result.Select(x => x * 2);

        Assert.False(mappedResult.IsSuccess);
        Assert.Single(mappedResult.Errors);
        Assert.Equal(error, mappedResult.Errors.First());
    }

    #endregion

    #region SelectMany Tests

    private static Result<int, FunqError> DoubleIfPositive(int number) =>
        number > 0
            ? Ok(number * 2)
            : Fail(new FunqError("NEGATIVE", "Number must be positive"));

    [Fact]
    public void SelectMany_ShouldFlattenNestedResults_OnSuccess()
    {
        var result = Ok(10);

        var flattenedResult = result.SelectMany(DoubleIfPositive);

        Assert.True(flattenedResult.IsSuccess);
        Assert.Equal(20, flattenedResult.Value);
    }

    [Fact]
    public void SelectMany_ShouldPropagateErrors_IfInitialResultIsFailure()
    {
        var error = new FunqError("INVALID", "Initial failure");
        var result = Fail(error);

        var flattenedResult = result.SelectMany(DoubleIfPositive);

        Assert.False(flattenedResult.IsSuccess);
        Assert.Single(flattenedResult.Errors);
        Assert.Equal(error, flattenedResult.Errors.First());
    }

    [Fact]
    public void SelectMany_ShouldPropagateErrors_IfSecondOperationFails()
    {
        var result = Ok(-5);

        var flattenedResult = result.SelectMany(DoubleIfPositive);

        Assert.False(flattenedResult.IsSuccess);
        Assert.Single(flattenedResult.Errors);
        Assert.Equal("NEGATIVE", flattenedResult.Errors.First().Code);
    }

    #endregion

    #region LINQ Query Syntax Tests

    [Fact]
    public void LinqQuery_ShouldWorkWithResultType()
    {
        Result<User, FunqError> GetUserById(int id) =>
            id == 1
                ? Result<User, FunqError>.Ok(new User("John", "john@example.com", true))
                : Result<User, FunqError>.Fail(new FunqError("NOT_FOUND", "User not found."));

        var accountResult = from user in GetUserById(1)
            where user.IsActive
            select new Account(user.Name, user.Email);

        Assert.True(accountResult.IsSuccess);
        Assert.Equal("John", accountResult.Value.Name);
    }

    [Fact]
    public void LinqQuery_ShouldHandleFailurePaths()
    {
        Result<User, FunqError> GetUserById(int id) =>
            id == 1
                ? Result<User, FunqError>.Ok(new User("John", "john@example.com", false))
                : Result<User, FunqError>.Fail(new FunqError("NOT_FOUND", "User not found."));

        var accountResult = GetUserById(1)
            .Where(user => user.IsActive, new FunqError("INACTIVE", "User is inactive."))
            .Select(user => new Account(user.Name, user.Email));

        Assert.False(accountResult.IsSuccess);
        Assert.Single(accountResult.Errors);
        Assert.Equal("User is inactive.", accountResult.Errors.First().Message);
    }

    #endregion
}

public record User(string Name, string Email, bool IsActive);
public record Account(string Name, string Email);
