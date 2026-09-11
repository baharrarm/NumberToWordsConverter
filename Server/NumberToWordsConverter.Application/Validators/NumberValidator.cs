namespace NumberToWordsConverter.Application.Validators;

public class NumberValidator
{
    /// <summary>
    ///     Gets a decimal number and checks to see if it can be converted.
    ///     If not, it returns the error message.
    ///     If yes, it returns an empty string.
    /// </summary>
    public string Validate(decimal number)
    {
        decimal fraction = number - decimal.Truncate(number);

        if (number <= 0) 
        {
            return "The number must be greater than zero.";
        }
        else if (decimal.Truncate(number) > (decimal)ulong.MaxValue) 
        {
            return "The number is too big.";
        }
        else if (!decimal.IsInteger(fraction * 100)) // Whole cents are valid because trailing zeros (like 12.3400) don't change precision.
        {
            return "The fraction must not be more than 2 digits.";
        }
        else 
        {
            return "";
        }
    }
}