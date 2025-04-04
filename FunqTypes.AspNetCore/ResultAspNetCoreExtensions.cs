using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace FunqTypes.AspNetCore;

public static class ResultAspNetCoreExtensions
{
    /// <summary>
    /// Converts <see cref="Result{T, E}"/> to IActionResult with a customizable success status code.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <param name="successStatusCode">The HTTP status code for success (default: 200 OK).</param>
    /// <param name="errorStatusMapper">Function to map errors to HTTP status codes (default: 400 Bad Request).</param>
    public static IActionResult ToActionResult<T, E>(
        this Result<T, E> result,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        Func<E, HttpStatusCode>? errorStatusMapper = null)
    {
        if (result.IsSuccess)
        {
            return new ObjectResult(new ResponseData<T>(result.Value)) { StatusCode = (int)successStatusCode };
        }

        var statusCode = errorStatusMapper != null && result.Errors.Count != 0
            ? (int)errorStatusMapper(result.Errors.First())
            : (int)HttpStatusCode.BadRequest;

        return new ObjectResult(new ErrorResponse<E>(result.Errors)) { StatusCode = statusCode };
    }

    /// <summary>
    /// Converts async <see cref="Result{T, E}"/> to <see cref="Task{IActionResult}"/> for controller actions.
    /// </summary>
    public static async Task<IActionResult> ToActionResultAsync<T, E>(
        this Task<Result<T, E>> resultTask,
        HttpStatusCode successStatusCode = HttpStatusCode.OK,
        Func<E, HttpStatusCode>? errorStatusMapper = null)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.ToActionResult(successStatusCode, errorStatusMapper);
    }
}

/// <summary>
/// Wrapper for successful API responses.
/// </summary>
public record ResponseData<T>(T Data);

/// <summary>
/// Wrapper for error API responses.
/// </summary>
public record ErrorResponse<E>(List<E> Errors);
