# <img src="icon.png" alt="FunqTypes Logo" width="45"/> FunqTypes

*A lightweight functional types for C#.*

[![Build](https://github.com/dnovhorodov/FunqTypes/actions/workflows/build-and-publish.yml/badge.svg)](https://github.com/dnovhorodov/FunqTypes/actions/workflows/build-and-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/FunqTypes.svg)](https://www.nuget.org/packages/FunqTypes/)
[![Downloads](https://img.shields.io/nuget/dt/FunqTypes.svg)](https://www.nuget.org/packages/FunqTypes/)
[![License](https://img.shields.io/github/license/dnovhorodov/FunqTypes.svg)](https://github.com/dnovhorodov/FunqTypes/blob/main/LICENSE)

---

## 🎯 **What is FunqTypes?**
**FunqTypes** is a set of **functional types and helpers for .NET** that brings **null-safety, error handling and composition** to C# applications. It provides:
- ✅ **`Result<T>` type** - Monadic error handling with `Yeah/Nah` aliases
- ✅ **`Option<T>` type** - Null-safe optional values with `Yeah/Nah` aliases
- ✅ **LINQ & FP Support** - Fluent mappings, binding, filtering
- ✅ **Zero Overhead** - Efficient `readonly record struct` implementation

> **Why?**  
> Writing clean, composable, and bug-resistant code in C# shouldn't be painful. **FunqTypes** helps by eliminating exceptions as a control flow mechanism and replacing them with functional, **explicit** error handling and null-safe optional values to avoid Null Reference Exceptions.

---

## 📦 **Installation**
### **.NET CLI**
```sh
dotnet add package FunqTypes
```

## 🚀 **Option<T> - Null-Safe Optional Values**
`Option<T>` represents an optional value that is either:
* `Some(T)` – Contains a value ✅
* `None` – Represents the absence of a value ❌

### ✅ Create an Option
```csharp
var someValue = Option<int>.Some(42);
var noneValue = Option<int>.None();
```
### Or use the Funq-style aliases 🤘:
```csharp
var funky = Option<int>.Yeah(42); // Same as Some(42)
var empty = Option<int>.Nah();   // Same as None()
```

### 🔄 Implicit Conversions
```csharp
Option<string> implicitSome = "FunqTypes"; // Automatically converts to Some("FunqTypes")
Option<string> implicitNone = null;        // Automatically converts to None()
```
**✔ No need to manually check for null anymore!**

### 🎯 Accessing Values
```csharp
var option = Option<int>.Yeah(10);

Console.WriteLine(option.GetValueOrDefault());        // Output: 10
Console.WriteLine(option.GetValueOrDefault(100));    // Output: 10
Console.WriteLine(Option<int>.Nah().GetValueOrDefault(100)); // Output: 100
```

**✔ Supports custom defaults and lazy evaluation:**
```csharp
var fallback = Option<int>.Nah().GetValueOrDefault(() => ComputeExpensiveValue());
```

### 🛠️ Functional Methods

**✅ Map – Transform an Option**
```csharp
var option = Option<int>.Yeah(5);
var doubled = option.Map(x => x * 2);

Console.WriteLine(doubled.GetValueOrDefault()); // Output: 10
```

**✔ If None, mapping does nothing:**
```csharp
Option<int>.Nah().Map(x => x * 2); // Stays None
```

**✅ Bind – Flatten Nested Options**
```csharp
Option<int> Parse(string input) =>
    int.TryParse(input, out var num) ? Option<int>.Yeah(num) : Option<int>.Nah();

var result = Option<string>.Yeah("42").Bind(Parse);

Console.WriteLine(result.GetValueOrDefault()); // Output: 42
```

### 🔍 Filtering with Where
```csharp
var option = Option<int>.Yeah(10);
var filtered = option.Where(x => x > 5);  // Stays Some(10)
var noneFiltered = option.Where(x => x > 15); // Becomes None()

Console.WriteLine(filtered.IsSome); // True
Console.WriteLine(noneFiltered.IsNone); // True
```

### 🖥️ Executing Side Effects with IfYeah and IfNone
```csharp
var option = Option<string>.Yeah("FunqTypes Rocks!");

option.IfYeah(Console.WriteLine); // Output: "FunqTypes Rocks!"
option.IfNah(() => Console.WriteLine("No value found.")); // Not called
```

**✔ Works even if the option is empty:**
```csharp
Option<string>.Nah().IfNah(() => Console.WriteLine("No value found.")); // Output: "No value found."
```

### 📦 Convert Option<T> to Result<T, E>
```csharp
var optionalData = Option<int>.Nah();
var result = optionalData.ToResult("Value not found");

Console.WriteLine(result.IsNeat); // False
Console.WriteLine(result.Errors.First()); // Output: "Value not found"
```

### 🔁 ToEnumerable() for LINQ Support
```csharp
var option = Option<int>.Yeah(5);
var sum = option.ToEnumerable()
                .Select(x => x * 2)
                .Sum();

Console.WriteLine(sum); // Output: 10
```

**✔ Empty Option<T> results in an empty sequence:**
```csharp
var sum = Option<int>.Nah().ToEnumerable().Sum(); // 0
```

### 🚀 Option<T> API Summary
| Method                             | Description                                |
|------------------------------------|--------------------------------------------|
| `Option<T>.Some(value)`            | Creates an Option containing value         |
| `Option<T>.None()`                 | Represents the absence of a value          |
| `.Yeah(value)`                     | Alias for `Some(value)`                    |
| `.Nah()`                           | Alias for `None() `                        |
| `.GetValueOrDefault()`             | Returns the value or `default(T)`          |
| `.GetValueOrDefault(fallback)`     | Returns the value or fallback              |
| `.Map(func)`                       | Transforms `Option<T>` into `Option<U>`    |
| `.Bind(func)`                      | Maps `Option<T>` to another `Option<U>`    |
| `.Where(predicate)`                | Filters `Option<T>`                        |
| `IfSome(action)`,`.IfYeah(action)` | Executes action if `Some`                  |
| `IfNone(action)`,`.IfNah(action)`  | Executes action if `None`                  |
| `ToResult(error)`                  | Converts `Option<T>` to `Result<T, E>`     |
| `ToEnumerable()`                   | Converts `Option<T>` into `IEnumerable<T>` |

---

## 🚀 **Result<T, E> - Handle Success or Error**
`Result<T, E>` represents returned value or an error:

* `T` – Represents success value ✅
* `E` – Represents error ❌

### ✅ Basic Success & Failure
```csharp
public record FunqError(string Code, string Message);

var success = Result<int, FunqError>.Yeah(42);
var failure = Result<int, FunqError>.Nah(new FunqError("INVALID", "Invalid input"));

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
        : Result<User, FunqError>.Nah(validationResult.Errors.ToArray());
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

### 🔄 Filtering with Where
**✅ Where(predicate, error)**

Filters a successful `Result<T, E>` based on a predicate.

* If predicate(value) is false, it transforms the success into a failure with the specified error.
* If the Result was already a failure, it remains unchanged.

```csharp
var result = Result<int, string>.Yeah(10)
    .Where(x => x > 5, "Value must be greater than 5");

Console.WriteLine(result.IsNeat); // True

var failed = Result<int, string>.Yeah(3)
    .Where(x => x > 5, "Value must be greater than 5");

Console.WriteLine(failed.IsNeat); // False
Console.WriteLine(failed.Errors.First()); // "Value must be greater than 5"
```
**✔ If Result is already a failure, Where does nothing:**
```csharp
var alreadyFailed = Result<int, string>.Nah("Initial failure")
    .Where(x => x > 5, "New failure");

Console.WriteLine(alreadyFailed.Errors.First()); // "Initial failure"
```

**✅ Where(predicate)**

Same as above, but uses a default error `(default(E))` if predicate(value) fails.
```csharp
var filtered = Result<int, string>.Yeah(10)
    .Where(x => x > 15); // Becomes failure (default error)

Console.WriteLine(filtered.IsYeah); // False
Console.WriteLine(filtered.Errors.Count); // 1
```

### 🔄 Transforming with Select

**✅ Select(selector)**
Applies a function to a successful result, transforming `T` into `U`.
```csharp
var result = Result<int, string>.Yeah(5)
    .Select(x => x * 2);

Console.WriteLine(result.IsNeat); // True
Console.WriteLine(result.Value); // 10
```
**✔ If the Result is already a failure, Select does nothing:**

```chsarp
var failed = Result<int, string>.Nah("Invalid number")
    .Select(x => x * 2);

Console.WriteLine(failed.IsNeat); // False
Console.WriteLine(failed.Errors.First()); // "Invalid number"
```

### 🔄 Composing with SelectMany
**✅ SelectMany(binder)**

Allows chaining operations where `T → Result<U, E>`.
```csharp
Result<int, string> Parse(string input) =>
    int.TryParse(input, out var num) ? Result<int, string>.Yeah(num) : Result<int, string>.Nah("Invalid number");

Result<int, string> EnsurePositive(int number) =>
    number > 0 ? Result<int, string>.Yeah(number) : Result<int, string>.Nah("Must be positive");

var result = Result<string, string>.Yeah("42")
    .SelectMany(Parse)
    .SelectMany(EnsurePositive);

Console.WriteLine(result.IsNeat); // True
Console.WriteLine(result.Value); // 42
```
**✔ Handles failure propagation automatically:**
```csharp
var failed = Result<string, string>.Yeah("-10")
    .SelectMany(Parse)
    .SelectMany(EnsurePositive);

Console.WriteLine(failed.IsNeat); // False
Console.WriteLine(failed.Errors.First()); // "Must be positive"
```

### 🚀 Result<T, E> API Summary
| Method                                                                   | Description                                               |
|--------------------------------------------------------------------------|-----------------------------------------------------------|
| `Result<T, E>.Ok(value)`                                                 | Creates a successful result                               |
| `Result<T, E>.Fail(errors...)`                                           | Creates a failed result with one or more errors           |
| `.Yeah(value)`                                                           | Alias for `.Ok(value)`                                    |
| `.Nah(errors...)`                                                        | Alias for `Fail(errors...)`                               |
| `.Bind(func)`                                                            | Chains operations, stopping on first failure              |
| `.Map(func)`                                                             | Transforms a success value                                |
| `.Ensure(predicate, error)`                                              | Validates a success value, failing if predicate is false  |
| `.Combine(results...)`                                                   | Combines multiple results, accumulating errors            |
| `.Match(onSuccess, onFailure)`                                           | Pattern matches on success or failure                     |
| `.BindAsync(func), .MapAsync(func), .EnsureAsync(predicate, error)`      | Asynchronous versions of Bind, Map, and Ensure            |
| `.GetValueOrDefault(), .GetValueOrDefault(T), ..GetValueOrDefault(func)` | Returns success value or default                          |
| `.Where(predicate, error)`                                               | Filters `Result<T, E>`, failing if predicate is false     |
| `.Where(predicate)`                                                      | Filters without specifying an error (uses `default(E)`)   |
| `.Select(predicate)`                                                     | Transforms `T → U` while keeping `Result<T, E>` structure |
| `.SelectMany(binder)`                                                    | Chains multiple `Result<T, E>` computations               |

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
        : Result<int>.Nah("Invalid number format.");

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
        errors => $"User creation failed: {string.Join("; ", errors)}"
    );
}

// Test cases:
Console.WriteLine(ProcessUserInput("User", "Password1")); // Username too short
Console.WriteLine(ProcessUserInput("ValidUser", "pass")); // Password too short
Console.WriteLine(ProcessUserInput("ValidUser", "ValidPass1")); // User successfully created
```

🔥 Star the repo if you find this useful! ⭐