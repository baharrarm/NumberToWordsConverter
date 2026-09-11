using NumberToWordsConverter.Application.Constants;
using NumberToWordsConverter.Application.Models;
using NumberToWordsConverter.Application.Services.Interfaces;
using NumberToWordsConverter.Application.Validators;

namespace NumberToWordsConverter.Application.Services;

public class NumberConverterService : INumberConverterService
{
    private readonly NumberValidator _validator = new NumberValidator();
    private readonly string[] _currencyUnits = {"Dollar", "Cent"}; 

    /// <summary>
    ///     Gets a decimal number and converts it into words with currency details and combines them into a complete result in Uppercase. 
    ///     Returns a failed result with error message if validation fails. 
    /// </summary>
    public ConversionResult GenerateNumberWordsCompleteString(decimal number)
    {
        var error = _validator.Validate(number);

        if (!string.IsNullOrEmpty(error))
            return new ConversionResult(false, "", error);


        List<string> words = new List<string>();
        
        var integerPart = Convert.ToUInt64(Math.Truncate(number));
        var fractionPart = Convert.ToUInt64((number - integerPart) * 100);
        ulong[] numberParts = {integerPart, fractionPart};

        for (int i = 0; i < numberParts.Length; i++)
        {
            if (numberParts[i] > 0)
            {
                words.AddRange(GenerateNumberWords(numberParts[i]));
                words.Add(_currencyUnits[i] + (numberParts[i] > 1 ? 's' : string.Empty));

                if (i == 0 && numberParts[i + 1] > 0) 
                    words.Add("And");
            }
        }
        
        var result = string.Join(" ", words).ToUpperInvariant(); // To stay consistent on different computers
        return new ConversionResult(true, result, "");
    }

    /// <summary>
    ///     Gets an integer number (integer part of the main number or the fractions part) and converts it to words 
    /// </summary>
    private List<string> GenerateNumberWords(ulong number)
    {
        List<int> splittedNumber = SplitByNotations(number);
        List<string> words = new List<string>();
        
        bool needsAnd = true;

        for (int i = 0; i < splittedNumber.Count(); i++)
        {
            if (splittedNumber[i] != 0)
            {
                if (i == splittedNumber.Count() - 1)
                    needsAnd = false;
                
                words.InsertRange(0, Map3DigitNumberToWords(splittedNumber[i], i, needsAnd));   
            }

        }
        return words;
    }

    /// <summary>
    ///     Gets each part of the main number and splits the number into 3-digits parts
    /// </summary>
    private List<int> SplitByNotations(ulong number)
    {
        List<int> splittedNumber = new List<int>();
        do
        {
            // Groups store from smallest notation: 1234567 gets stored like [567, 234, 1].
            splittedNumber.Add((int)(number % 1000));
            number /= 1000;

        } while (number > 0);
        return splittedNumber;
    }

    /// <summary>
    ///     Gets a 3-digit part of the integer part of the main number and converts it to words with notations included
    /// </summary>
    private List<string> Map3DigitNumberToWords(int number, int notationIndex, bool needsAnd)
    {
        var threeDigitNumberWord = new List<string>();

        // Add "And" before each nonzero group except the last group (highest notation).
        if (needsAnd) 
            threeDigitNumberWord.Add("And");
        
        if (number / 100 > 0)
        {
            threeDigitNumberWord.Add(NumberWords.UnitsMap[(number / 100) - 1]);
            threeDigitNumberWord.Add(NumberWords.NotationsMap[0]);
            number %= 100;
            
            if (number > 0)
                threeDigitNumberWord.Add("And");
        }

        if (number > 0)
        {                
            if (number < 20) 
            {
                threeDigitNumberWord.Add(NumberWords.UnitsMap[number - 1]);
            }
            else 
            {
                string last2Digit =  NumberWords.TensMap[(number / 10) - 1];
        
                if ((number % 10) > 0)
                    last2Digit += "-" + NumberWords.UnitsMap[number % 10 - 1];

                threeDigitNumberWord.Add(last2Digit);
            }

        }

        if (notationIndex > 0) 
            threeDigitNumberWord.Add(NumberWords.NotationsMap[notationIndex]);
        
        return threeDigitNumberWord;
    }

    

}