namespace FunqTypes.Tests;

public class OptionTests
{
    #region Creation & Implicit Conversion

    [Fact]
    public void Some_ShouldCreateOptionWithValue()
    {
        var option = Some(42);

        Assert.True(option.IsSome);
        Assert.False(option.IsNone);
        Assert.Equal(42, option.GetValueOrDefault());
    }

    [Fact]
    public void None_ShouldCreateEmptyOption()
    {
        var option = None();

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
        Assert.Equal(0, option.GetValueOrDefault());
    }

    [Fact]
    public void ImplicitConversionFromValue_ShouldCreateSome()
    {
        Option<string> option = "FunqTypes";

        Assert.True(option.IsSome);
        Assert.Equal("FunqTypes", option.GetValueOrDefault());
    }

    [Fact]
    public void ImplicitConversionFromNull_ShouldCreateNone()
    {
        Option<string> option = null!;

        Assert.True(option.IsNone);
        Assert.Null(option.GetValueOrDefault());
    }

    [Fact]
    public void ImplicitConversionFromNoneType_ShouldCreateNone()
    {
        Option<int> option = Option.None;

        Assert.True(option.IsNone);
    }

    #endregion
    
    #region Funq Aliases (Yeah & Nope)

    [Fact]
    public void Yeah_ShouldCreateOptionWithValue()
    {
        var option = Option<int>.Yeah(42);

        Assert.True(option.IsSome);
        Assert.False(option.IsNone);
        Assert.Equal(42, option.GetValueOrDefault());

        var expected = Some(42);
        Assert.Equal(expected, option);
    }

    [Fact]
    public void Nah_ShouldCreateNoneOption()
    {
        var option = Nah();

        Assert.False(option.IsSome);
        Assert.True(option.IsNone);
        Assert.Equal(0, option.GetValueOrDefault());

        var expected = None();
        Assert.Equal(expected, option);
    }

    #endregion

    #region GetValueOrDefault

    [Fact]
    public void GetValueOrDefault_ShouldReturnContainedValue()
    {
        var option = Some(99);

        Assert.Equal(99, option.GetValueOrDefault());
    }

    [Fact]
    public void GetValueOrDefault_ShouldReturnDefaultValueForNone()
    {
        var option = None();

        Assert.Equal(0, option.GetValueOrDefault());
    }

    [Fact]
    public void GetValueOrDefault_WithFallback_ShouldReturnValueIfSome()
    {
        var option = Some(7);

        Assert.Equal(7, option.GetValueOrDefault(100));
    }

    [Fact]
    public void GetValueOrDefault_WithFallback_ShouldReturnFallbackIfNone()
    {
        var option = None();

        Assert.Equal(100, option.GetValueOrDefault(100));
    }

    [Fact]
    public void GetValueOrDefault_WithFactory_ShouldReturnValueIfSome()
    {
        var option = Some(25);

        Assert.Equal(25, option.GetValueOrDefault(() => 999));
    }

    [Fact]
    public void GetValueOrDefault_WithFactory_ShouldCallFactoryIfNone()
    {
        var option = None();
        var factoryCalled = false;

        var value = option.GetValueOrDefault(() =>
        {
            factoryCalled = true;
            return 888;
        });

        Assert.Equal(888, value);
        Assert.True(factoryCalled);
    }

    #endregion

    #region Map & Bind

    [Fact]
    public void Map_ShouldTransformSomeValue()
    {
        var option = Some(5);
        var result = option.Map(x => x * 2);

        Assert.True(result.IsSome);
        Assert.Equal(10, result.GetValueOrDefault());
    }

    [Fact]
    public void Map_ShouldReturnNoneIfNone()
    {
        var option = None();
        var result = option.Map(x => x * 2);

        Assert.True(result.IsNone);
    }

    [Fact]
    public void Bind_ShouldFlattenOptionResult()
    {
        Option<int> SomeTransform(int x) => Some(x * 2);

        var option = Some(3);
        var result = option.Bind(SomeTransform);

        Assert.True(result.IsSome);
        Assert.Equal(6, result.GetValueOrDefault());
    }

    [Fact]
    public void Bind_ShouldReturnNoneIfNone()
    {
        Option<int> SomeTransform(int x) => Some(x * 2);

        var option = None();
        var result = option.Bind(SomeTransform);

        Assert.True(result.IsNone);
    }

    #endregion

    #region IfSome & IfNone

    [Fact]
    public void IfSome_ShouldExecuteActionIfSome()
    {
        var option = Option<string>.Some("Hello");
        var called = false;

        option.IfSome(value =>
        {
            Assert.Equal("Hello", value);
            called = true;
        });

        Assert.True(called);
    }

    [Fact]
    public void IfNone_ShouldExecuteActionIfNone()
    {
        var option = Option<string>.None();
        var called = false;

        option.IfNone(() =>
        {
            called = true;
        });

        Assert.True(called);
    }

    [Fact]
    public void IfYeah_ShouldBehaveExecuteLike_IfSome()
    {
        var option = Option<int>.Yeah(42);
        var yeahCalled = false;
        var someCalled = false;

        option.IfYeah(_ => yeahCalled = true);
        option.IfSome(_ => someCalled = true);

        Assert.Equal(yeahCalled, someCalled);
    }
    
    [Fact]
    public void IfNah_ShouldBehaveExecuteLike_IfNone()
    {
        var option = Nah();
        var nahCalled = false;
        var noneCalled = false;

        option.IfNah(() => nahCalled = true);
        option.IfNone(() => noneCalled = true);

        Assert.Equal(nahCalled, noneCalled);
    }

    #endregion

    #region Where

    [Fact]
    public void Where_ShouldReturnSameOptionIfPredicateIsTrue()
    {
        var option = Some(10);
        var result = option.Where(x => x > 5);

        Assert.True(result.IsSome);
        Assert.Equal(10, result.GetValueOrDefault());
    }

    [Fact]
    public void Where_ShouldReturnNoneIfPredicateIsFalse()
    {
        var option = Some(3);
        var result = option.Where(x => x > 5);

        Assert.True(result.IsNone);
    }

    [Fact]
    public void Where_ShouldNotAffectNone()
    {
        var option = None();
        var result = option.Where(x => x > 5);

        Assert.True(result.IsNone);
    }

    #endregion

    #region ToResult

    [Fact]
    public void ToResult_ShouldConvertSomeToSuccess()
    {
        var option = Some(42);
        var result = option.ToResult("No value available");

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void ToResult_ShouldConvertNoneToFailure()
    {
        var option = None();
        var result = option.ToResult("No value available");

        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("No value available", result.Errors.First());
    }

    #endregion

    #region ToEnumerable

    [Fact]
    public void ToEnumerable_ShouldReturnSingleElementForSome()
    {
        var option = Some(7);
        var list = option.ToEnumerable().ToList();

        Assert.Single(list);
        Assert.Equal(7, list.First());
    }

    [Fact]
    public void ToEnumerable_ShouldReturnEmptyForNone()
    {
        var option = None();
        var list = option.ToEnumerable().ToList();

        Assert.Empty(list);
    }

    #endregion
}
