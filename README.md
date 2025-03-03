# <img src="icon.png" alt="FunqSharp Logo" width="45"/> Funq♯

*A lightweight functional programming library for C#.*

---

## 🎯 **What is FunqSharp?**
**FunqSharp** is a **functional programming library for .NET** that brings **functional error handling and composition** to C# applications. It provides:
- ✅ A **lightweight `Result<T>` type** for functional error handling
- 🔥 **Primary API:** `Yeah()`, `Nope()`, and `IsNeat` → Short, fun, and easy to read
- ✅ **Alternative API:** `Ok()`, `Fail()`, and `IsSuccess` → For devs who prefer formal names
- ✅ **Monadic (`Bind`, `Map`)** and **Applicative (`Combine`)** styles
- ✅ **Asynchronous (`BindAsync`, `MapAsync`)** support
- ✅ **Pattern matching with `Match`**
- ✅ **Immutability and safety using `record struct`**

> **Why?**  
> Writing clean, composable, and bug-resistant code in C# shouldn't be painful. **FunqSharp** helps by eliminating exceptions as a control flow mechanism and replacing them with functional, **explicit** error handling.

---

## 📦 **Installation**
### **.NET CLI**
```sh
dotnet add package FunqSharp
```

## 🚀 **Getting Started**

### ✅ Basic Success & Failure
```csharp
public record FunqError(string Code, string Message);

var success = Result<int, FunqError>.Yeah(42);
var failure = Result<int, FunqError>.Nope(new FunqError("INVALID", "Invalid input"));

Console.WriteLine(success.IsNeat); // True
Console.WriteLine(failure.IsNeat); // False
```

### 🔄 Monadic Composition (Fail-Fast Approach)
```csharp
public static Result<User, FunqError> CreateUser(string username, string email, string password)
{
    return ValidateUsername(username)
        .Bind(_ => ValidateEmail(email))
        .Bind(_ => ValidatePassword(password))
        .Map(_ => new User(username, email, password));
}
```

### 📌 Applicative Validation (Accumulate Errors)
```csharp
public static Result<User, FunqError> CreateUserAggregated(string username, string email, string password)
{
    var usernameResult = ValidateUsername(username);
    var emailResult = ValidateEmail(email);
    var passwordResult = ValidatePassword(password);

    var validationResult = Result<string, FunqError>.Combine(usernameResult, emailResult, passwordResult);

    return validationResult.IsSuccess
        ? Result<User, FunqError>.Yeah(new User(username, email, password))
        : Result<User, FunqError>.Nope(validationResult.Errors.ToArray());
}
```

### ⚡ Asynchronous Support
```csharp
public async Task<Result<User, FunqError>> CreateUserAsync(string username, string email, string password)
{
    return await ValidateUsernameAsync(username)
        .BindAsync(ValidateEmailAsync)
        .BindAsync(ValidatePasswordAsync)
        .MapAsync(user => new User(username, email, password));
}
```

### 🎭 Pattern Matching
```csharp
var result = CreateUser("JohnDoe", "invalidemail", "123");

var message = result.Match(
    success => $"User created: {success.Name}",
    errors => $"Failed: {string.Join(", ", errors)}"
);

Console.WriteLine(message);
```

## API Overview
| Method                                                              | Description                                                   |
|---------------------------------------------------------------------|---------------------------------------------------------------|
| `Result<T, E>.Yeah(value)`                                           | Creates a successful result -                                 |
| `Result<T, E>.Nope(errors...)`                                      | Creates a failed result with one or more<br/> errors          |
| `.Bind(func)`                                                       | Chains operations, stopping on first failure                  |
| `.Map(func)`                                                        | Transforms a success value                                    |
| `.Ensure(predicate, error)`                                         | Validates a success value, failing if predicate<br/> is false |
| `.Combine(results...)`                                              | Combines multiple results, accumulating<br/> errors           |
| `.Match(onSuccess, onFailure)`                                      | Pattern matches on success or failure                         |
| `.BindAsync(func), .MapAsync(func), .EnsureAsync(predicate, error)` | Asynchronous versions of Bind, Map, and Ensure                |

### When to Use Each?
| Use Case                                                                 | Best Method                    |
|--------------------------------------------------------------------------|--------------------------------|
| Validate a field and fail if invalid                                     | `Ensure(condition, error)`     | 
| Transform a successful result                                            | `Map(func)`                    | 
| Chain operations where the next step<br/>depends on the previous success | `Bind(func)`                   | 
| Collect multiple validation errors                                       | `Ensure` multiple times        | 
| Handle both success and failure explicitly                               | `Match(onSuccess, onFailure)`  | 

## More examples

### 🔹 Combining Ensure, Bind, and Map
```csharp
public static Result<int, string> ParseNumber(string input) =>
    int.TryParse(input, out int number)
        ? Result<int>.Yeah(number)
        : Result<int>.Nope("Invalid number format.");

var result = Result<string, string>.Ok("42")
    .Bind(ParseNumber)
    .Ensure(num => num > 0, "Number must be positive.")
    .Map(num => $"Processed number: {num * 2}");

var message = result.Match(
    success => success,
    errors => $"Processing failed: {string.Join(", ", errors)}"
);

Console.WriteLine(message); // Output: "Processed number: 84"
```

### 🔹 Aggregating Multiple Validations Using Ensure
```csharp
var result = Result<string, string>.Yeah("Ad")
    .Ensure(name => name.Length >= 3, "Name must be at least 3 characters.")
    .Ensure(name => name.All(char.IsLetterOrDigit), "Name must contain only letters and digits.")
    .Ensure(name => !name.StartsWith("Admin"), "Name cannot start with 'Admin'.");

var message = result.Match(
    success => $"Username '{success}' is valid!",
    errors => $"Username validation failed: {string.Join("; ", errors)}"
);

Console.WriteLine(message);
// Output: "Username validation failed: Name must be at least 3 characters."
```

### 🔹 Using Ensure and Math in API-Like Scenario
```csharp
public static string ProcessUserInput(string username, string password)
{
    var result = Result<string, string>.Yeah(username)
        .Ensure(u => u.Length >= 5, "Username must be at least 5 characters.")
        .Ensure(u => !u.Contains(" "), "Username cannot contain spaces.")
        .Ensure(u => char.IsLetter(u[0]), "Username must start with a letter.")
        .Bind(_ => Result<string>.Yeah(password))
        .Ensure(p => p.Length >= 8, "Password must be at least 8 characters.")
        .Ensure(p => p.Any(char.IsDigit), "Password must contain at least one number.")
        .Ensure(p => p.Any(char.IsUpper), "Password must contain at least one uppercase letter.");

    return result.Match(
        success => "User successfully created ✅",
        errors => $"User creation failed ❌: {string.Join("; ", errors)}"
    );
}

// Test cases:
Console.WriteLine(ProcessUserInput("User", "Password1")); // ❌ Username too short
Console.WriteLine(ProcessUserInput("ValidUser", "pass")); // ❌ Password too short
Console.WriteLine(ProcessUserInput("ValidUser", "ValidPass1")); // ✅ User successfully created
```

🔥 Star the repo if you find this useful! ⭐