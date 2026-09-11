using NumberToWordsConverter.Application.Models;

namespace NumberToWordsConverter.Application.Services.Interfaces;

public interface INumberConverterService
{
    ConversionResult GenerateNumberWordsCompleteString(decimal number);
    
}