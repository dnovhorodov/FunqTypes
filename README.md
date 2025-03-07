# <img src="icon.png" alt="FunqSharp Logo" width="45"/> Funq♯

*A lightweight functional programming library for C#.*

---

## 🎯 **What is FunqSharp?**
**FunqSharp** is a **functional programming library for .NET** that brings **functional error handling and composition** to C# applications. It provides:
- ✅ **`Result<T>` type** - Monadic error handling with `Yeah/Nah` aliases
- ✅ **`Option<T>` type** - Null-safe optional values with `Yeah/Nah` aliases
- ✅ **LINQ & FP Support** - Fluent mappings, binding, filtering
- ✅ **Zero Overhead** - Efficient `readonly record struct` implementation

> **Why?**  
> Writing clean, composable, and bug-resistant code in C# shouldn't be painful. **FunqSharp** helps by eliminating exceptions as a control flow mechanism and replacing them with functional, **explicit** error handling and null-safe optional values to avoid Null Reference Exceptions.

---

## 📦 **Installation**
### **.NET CLI**
```sh
dotnet add package FunqSharp
```

## 🚀 **Option<T> - Null-Safe Optional Values**
Option<T> represents an optional value that is either:
* **Some(T)** – Contains a value ✅
* **None** – Represents the absence of a value ❌

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
Option<string> implicitSome = "FunqSharp"; // Automatically converts to Some("FunqSharp")
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
var option = Option<string>.Yeah("FunqSharp Rocks!");

option.IfYeah(Console.WriteLine); // Output: "FunqSharp Rocks!"
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

---

## 🚀 **Result<T, E> - Handle Success or Error**

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

### 🚀 Result<T, E> API Summary
| Method                                                                   | Description                                                   |
|--------------------------------------------------------------------------|---------------------------------------------------------------|
| `Result<T, E>.Yeah(value)`                                               | Creates a successful result -                                 |
| `Result<T, E>.Nah(errors...)`                                           | Creates a failed result with one or more<br/> errors          |
| `.Bind(func)`                                                            | Chains operations, stopping on first failure                  |
| `.Map(func)`                                                             | Transforms a success value                                    |
| `.Ensure(predicate, error)`                                              | Validates a success value, failing if predicate<br/> is false |
| `.Combine(results...)`                                                   | Combines multiple results, accumulating<br/> errors           |
| `.Match(onSuccess, onFailure)`                                           | Pattern matches on success or failure                         |
| `.BindAsync(func), .MapAsync(func), .EnsureAsync(predicate, error)`      | Asynchronous versions of Bind, Map, and Ensure                |
| `.GetValueOrDefault(), .GetValueOrDefault(T), ..GetValueOrDefault(func)` | Returns success value or default                              |

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