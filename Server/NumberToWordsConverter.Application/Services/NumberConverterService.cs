using NumberToWordsConverter.Application.Services.Interfaces;

namespace NumberToWordsConverter.Application.Services
{
    public class NumberConverterService : INumberConverterService
    {
        /// <summary>
        ///     Gets a decimal number and converts it into words with currency details and combines them into a complete result in Uppercase.  
        /// </summary>
        public string GenerateNumberWordsCompleteString(decimal number)
        {
            // Validate input number before use.
            return "";
        }

        /// <summary>
        ///     Gets a number without fractions (integer part of the main number or the fractions part) and converts it to words 
        /// </summary>
        private List<string> GenerateNumberWords(UInt64 number)
        {
            return [];
        }

        /// <summary>
        ///     Gets the integer part of the main number and splits the number into 3-digits parts
        /// </summary>
        private List<int> SplitByNotations(UInt64 number)
        {
            return [];
        }

        /// <summary>
        ///     Gets a 3-digit part of the integer part of the main number and converts it to words with notations included
        /// </summary>
        private List<string> Map3DigitNumberToWords(int number)
        {
            return [];
        }

        

    }
}