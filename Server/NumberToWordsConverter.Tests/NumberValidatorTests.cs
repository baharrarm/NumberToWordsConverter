using System.Globalization;
using NumberToWordsConverter.Application.Validators;

namespace NumberToWordsConverter.Tests;

public class NumberValidatorTests
{
    [Theory]
    [InlineData("18446744073709551615.99", "")]
    [InlineData("0.01", "")]
    [InlineData(".25", "")]
    [InlineData("1.2300", "")]
    [InlineData("46", "")]
    public void Succeed_WhenNumberIsValid(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); //to stay consistent on different computers
        string actual = new NumberValidator().Validate(num);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ReturnError_WhenNumberIsZero()
    {
        var validator = new NumberValidator();
        var error = validator.Validate(0m);
        Assert.Equal("The number must be greater than zero.", error);
    }

    [Fact]
    public void ReturnError_WhenNumberIsTooBig()
    {
        var validator = new NumberValidator();
        var error = validator.Validate(18446744073709551616m);
        Assert.Equal("The number is too big.", error);
    }

    [Fact]
    public void ReturnError_WhenNumberIsNegative()
    {
        var validator = new NumberValidator();
        var error = validator.Validate(-1.10m);
        Assert.Equal("The number must be greater than zero.", error);
    }


    [Theory]
    [InlineData("18446744073709551615.991", "The fraction must not be more than 2 digits.")]
    [InlineData("0.001", "The fraction must not be more than 2 digits.")]
    [InlineData("1.234", "The fraction must not be more than 2 digits.")]
    public void ReturnError_WhenFractionMoreThan2Digits(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture);
        string actual = new NumberValidator().Validate(num);
        Assert.Equal(expected, actual);
    }
}