## 🌐 FunqTypes.AspNet - Helpers for Asp.Net

## 🎯 **What is FunqTypes?**
**FunqTypes** is a set of **functional types and helpers for .NET** that brings **null-safety, error handling and composition** to C# applications. It provides:
- ✅ **`Result<T>` type** - Monadic error handling with `Ok/Fail` or `Gotcha/Oops` alternative aliases
- ✅ **`Option<T>` type** - Null-safe optional values with `Some/None` or `Yeah/Nah` alternative aliases
- ✅ **LINQ & FP Support** - Fluent mappings, binding, filtering
- ✅ **Zero Overhead** - Efficient `readonly record struct` implementation

### Standard Usage (Default `200 OK`)
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetAccount(int id)
{
    return await GetAccountById(id).ToActionResultAsync();
}
```
* Returns 200 OK on success.
* Returns 400 Bad Request on failure.

### `201 Created` for POST Requests
```csharp
[HttpPost]
public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
{
    return await CreateAccount(request).ToActionResultAsync(HttpStatusCode.Created);
}
```
* Returns 201 Created on success.
* Returns 400 Bad Request on failure.

### Custom Error Mapping with `HttpStatusCode`
```csharp
private static HttpStatusCode MapErrorToStatusCode(ValidationError error) =>
    error.Code switch
    {
        "NOT_FOUND" => HttpStatusCode.NotFound,
        "INVALID_INPUT" => HttpStatusCode.UnprocessableEntity,
        "UNAUTHORIZED" => HttpStatusCode.Unauthorized,
        _ => HttpStatusCode.BadRequest
    };
```
Then, use it in the controller:
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountRequest request)
{
    return await UpdateAccount(id, request).ToActionResultAsync(HttpStatusCode.OK, MapErrorToStatusCode);
}
```
* 404 Not Found for `NOT_FOUND`
* 422 Unprocessable Entity for `INVALID_INPUT`
* 401 Unauthorized for `UNAUTHORIZED`
* 400 Bad Request for other errors