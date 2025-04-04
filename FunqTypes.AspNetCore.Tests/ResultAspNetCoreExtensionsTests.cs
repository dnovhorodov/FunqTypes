using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FunqTypes.AspNetCore.Tests;

public class ResultAspNetCoreExtensionsTests
{
    private record TestError(string Code, string Message);

    [Fact]
    public void ToActionResult_ShouldReturnOkObjectResult_WhenResultIsSuccess()
    {
        var result = Result<string, TestError>.Ok("hello world");

        var actionResult = result.ToActionResult();

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        var response = Assert.IsType<ResponseData<string>>(objectResult.Value);
        Assert.Equal("hello world", response.Data);
    }

    [Fact]
    public void ToActionResult_ShouldReturnCustomSuccessCode_WhenProvided()
    {
        var result = Result<string, TestError>.Ok("created!");

        var actionResult = result.ToActionResult(HttpStatusCode.Created);

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.Created, objectResult.StatusCode);
    }

    [Fact]
    public void ToActionResult_ShouldReturnBadRequest_WhenFailureAndNoMapper()
    {
        var result = Result<string, TestError>.Fail(new TestError("INVALID", "Something failed"));

        var actionResult = result.ToActionResult();

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        var errorResponse = Assert.IsType<ErrorResponse<TestError>>(objectResult.Value);
        Assert.Single((IEnumerable)errorResponse.Errors);
        Assert.Equal("INVALID", errorResponse.Errors.First().Code);
    }

    [Fact]
    public void ToActionResult_ShouldUseErrorMapper_WhenProvided()
    {
        var result = Result<string, TestError>.Fail(new TestError("FORBIDDEN", "No access"));

        HttpStatusCode ErrorMapper(TestError error) =>
            error.Code == "FORBIDDEN" ? HttpStatusCode.Forbidden : HttpStatusCode.BadRequest;

        var actionResult = result.ToActionResult(errorStatusMapper: ErrorMapper);

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
    }

    [Fact]
    public async Task ToActionResultAsync_ShouldReturnExpectedResult_WhenSuccess()
    {
        var resultTask = Task.FromResult(Result<int, string>.Ok(99));

        var actionResult = await resultTask.ToActionResultAsync();

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        var data = Assert.IsType<ResponseData<int>>(objectResult.Value);
        Assert.Equal(99, data.Data);
    }

    [Fact]
    public async Task ToActionResultAsync_ShouldReturnExpectedResult_WhenFailure()
    {
        var resultTask = Task.FromResult(Result<int, string>.Fail("error123"));

        var actionResult = await resultTask.ToActionResultAsync();

        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
        var errorData = Assert.IsType<ErrorResponse<string>>(objectResult.Value);
        Assert.Single((IEnumerable)errorData.Errors);
        Assert.Equal("error123", errorData.Errors.First());
    }
}
