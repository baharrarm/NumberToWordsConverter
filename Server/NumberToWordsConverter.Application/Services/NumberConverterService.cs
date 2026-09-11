using NumberToWordsConverter.Application.Constants;
using NumberToWordsConverter.Application.Models;
using NumberToWordsConverter.Application.Services.Interfaces;
using NumberToWordsConverter.Application.Validators;

namespace NumberToWordsConverter.Application.Services
{
    public class NumberConverterService : INumberConverterService
    {
        private readonly NumberValidator _validator = new NumberValidator();
        private readonly string[] currencyUnits = {"Dollar" , "Cent"}; 

        /// <summary>
        ///     Gets a decimal number and converts it into words with currency details and combines them into a complete result in Uppercase. 
        ///     Returns a failed result with error message if validation fails. 
        /// </summary>
        public ConversionResult GenerateNumberWordsCompleteString(decimal number)
        {
            // Validate input number before use.
            var error = _validator.Validate(number);
            if (!string.IsNullOrEmpty(error))
            {
                return new ConversionResult(false, "", error);
            }


            List<string> words = new List<string>();
            
            var integerPart = Convert.ToUInt64(Math.Truncate(number));
            var fractionPart = Convert.ToUInt64((number - integerPart) * 100);
            UInt64[] numberParts = {integerPart, fractionPart};

            for (int i = 0; i < numberParts.Length; i++)
            {
                if (numberParts[i] > 0)
                {
                    words.AddRange(GenerateNumberWords(numberParts[i]));
                    words.Add(currencyUnits[i] + (numberParts[i] > 1 ? 's' : string.Empty));
                    if (i == 0 && numberParts[i+1] > 0) words.Add("and");
                }
            }
            
            var result = String.Join(" ", words).ToUpper();

            return new ConversionResult(true, result, "");
        }

        /// <summary>
        ///     Gets a number without fractions (integer part of the main number or the fractions part) and converts it to words 
        /// </summary>
        private List<string> GenerateNumberWords(UInt64 number)
        {
            List<int> splittedNumber = SplitByNotations(number);
            List<string> words = new List<string>();
            
            bool needsAnd;
            for (int i = 0; i < splittedNumber.Count(); i++)
            {
                if(splittedNumber[i] != 0)
                {
                    needsAnd = false;
                    if (i == 0 && splittedNumber[i] < 100 && splittedNumber.Count > 1)
                    {
                        needsAnd = true;
                    }
                    words.InsertRange(0, Map3DigitNumberToWords(splittedNumber[i], i, needsAnd));   
                }

            }
            return words;
        }

        /// <summary>
        ///     Gets each part of the main number and splits the number into 3-digits parts
        /// </summary>
        private List<int> SplitByNotations(UInt64 number)
        {
            List<int> splittedNumber = new List<int>();
            do
            {
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
            
            if (number/100 > 0)
            {
                threeDigitNumberWord.Add(NumberWords.UnitsMap[(number / 100) - 1]);
                threeDigitNumberWord.Add(NumberWords.Notations[0]);
                needsAnd = true;
                number %= 100;
            }

            if (number > 0)
            {
                if (needsAnd) threeDigitNumberWord.Add("And");
                if (number < 20)
                {
                    threeDigitNumberWord.Add(NumberWords.UnitsMap[number - 1]);
                }
                else
                {
                    string last2Digit =  NumberWords.TensMap[(number / 10)-1];
                    if ((number % 10) > 0)
                    {
                        last2Digit += "-" + NumberWords.UnitsMap[number % 10 - 1];
                    }
                    threeDigitNumberWord.Add(last2Digit);
                }

            }

            if(notationIndex>0) threeDigitNumberWord.Add(NumberWords.Notations[notationIndex]);
            return threeDigitNumberWord;
        }

        

    }
}