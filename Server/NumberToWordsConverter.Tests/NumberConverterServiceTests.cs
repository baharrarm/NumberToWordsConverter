using System.Globalization;
using NumberToWordsConverter.Application.Services;

namespace NumberToWordsConverter.Tests;

public class NumberConverterServiceTest
{
    NumberConverterService _converterService = new NumberConverterService();

    [Theory]
    [InlineData("18446744073709551615.15", 
        "EIGHTEEN QUINTILLION FOUR HUNDRED AND FORTY-SIX QUADRILLION SEVEN HUNDRED AND FORTY-FOUR TRILLION SEVENTY-THREE BILLION SEVEN HUNDRED AND NINE MILLION FIVE HUNDRED AND FIFTY-ONE THOUSAND SIX HUNDRED AND FIFTEEN DOLLARS AND FIFTEEN CENTS")]
    [InlineData("6543212.34", "SIX MILLION FIVE HUNDRED AND FORTY-THREE THOUSAND TWO HUNDRED AND TWELVE DOLLARS AND THIRTY-FOUR CENTS")]
    [InlineData("100.30", "ONE HUNDRED DOLLARS AND THIRTY CENTS")]
    [InlineData("145.09", "ONE HUNDRED AND FORTY-FIVE DOLLARS AND NINE CENTS")]
    [InlineData("1.2300", "ONE DOLLAR AND TWENTY-THREE CENTS")]
    [InlineData("0.98", "NINETY-EIGHT CENTS")]
    [InlineData(".01", "ONE CENT")]
    [InlineData("1", "ONE DOLLAR")]
    [InlineData("1.01", "ONE DOLLAR AND ONE CENT")]
    [InlineData("11", "ELEVEN DOLLARS")]
    [InlineData("20", "TWENTY DOLLARS")]
    [InlineData("100", "ONE HUNDRED DOLLARS")]
    [InlineData("1000", "ONE THOUSAND DOLLARS")]
    [InlineData("5000001", "FIVE MILLION AND ONE DOLLARS")]
    public void ReturnWordResult_ConvertNumberToWordsInCurrency_WhenNumberIsValid(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); //to stay consistent on different computers
        var actual = _converterService.GenerateNumberWordsCompleteString(num);
        Assert.True(actual.IsSuccess);
        Assert.Equal(expected, actual.Words);
    }

    [Theory]
    [InlineData("18446744073709551620", "The number is too big.")]
    [InlineData("000.000", "The number must be greater than zero.")]
    [InlineData("00", "The number must be greater than zero.")]
    [InlineData(".00", "The number must be greater than zero.")]
    [InlineData("-123", "The number must be greater than zero.")]
    [InlineData("-0.12", "The number must be greater than zero.")]
    [InlineData("-1.12", "The number must be greater than zero.")]
    [InlineData(".00012", "The fraction must not be more than 2 digits.")]
    [InlineData("1.456", "The fraction must not be more than 2 digits.")]
    public void ReturnError_ConvertNumberToWordsInCurrency_WhenNumberIsNotValid(string number, string expected)
    {
        decimal num = Convert.ToDecimal(number, CultureInfo.InvariantCulture); //to stay consistent on different computers
        var actual = _converterService.GenerateNumberWordsCompleteString(num);
        Assert.False(actual.IsSuccess);
        Assert.Equal(expected, actual.Error);
        Assert.Equal("", actual.Words);
    }
}
