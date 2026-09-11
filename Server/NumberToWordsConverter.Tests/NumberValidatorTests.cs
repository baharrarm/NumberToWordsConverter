using System.Globalization;
using NumberToWordsConverter.Application.Validators;

namespace NumberToWordsConverter.Tests;

public class NumberValidatorTests
{
    private readonly NumberValidator _validator = new NumberValidator();

    [Theory]
    [InlineData("18446744073709551615.99", "")]
    [InlineData("0.01", "")]
    [InlineData(".25", "")]
    [InlineData("1.2300", "")]
    [InlineData("46", "")]
    public void Succeed_WhenNumberIsValid(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); // To stay consistent on different computers
        string actual = _validator.Validate(num);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("0", "The number must be greater than zero.")]
    [InlineData(".00", "The number must be greater than zero.")]
    [InlineData("0.0", "The number must be greater than zero.")]
    public void ReturnError_WhenNumberIsZero(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); // To stay consistent on different computers
        string actual = _validator.Validate(num);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("18446744073709551616", "The number is too big.")]
    [InlineData("79228162514264337593543950335", "The number is too big.")] // Decimal.MaxValue
    public void ReturnError_WhenNumberIsTooBig(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); // To stay consistent on different computers
        var error = _validator.Validate(num);
        Assert.Equal(expected, error);
    }

    [Theory]
    [InlineData("-1.10", "The number must be greater than zero.")]
    [InlineData("-79228162514264337593543950335", "The number must be greater than zero.")] // Decimal.MinValue
    public void ReturnError_WhenNumberIsNegative(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); // To stay consistent on different computers
        var error = _validator.Validate(num);
        Assert.Equal(expected, error);
    }


    [Theory]
    [InlineData("18446744073709551615.991", "The fraction must not be more than 2 digits.")]
    [InlineData("0.001", "The fraction must not be more than 2 digits.")]
    [InlineData("1.234", "The fraction must not be more than 2 digits.")]
    public void ReturnError_WhenFractionMoreThan2Digits(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture);
        string actual = _validator.Validate(num);
        Assert.Equal(expected, actual);
    }
}