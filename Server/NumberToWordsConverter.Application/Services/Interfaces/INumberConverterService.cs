namespace NumberToWordsConverter.Application.Services.Interfaces
{
    public interface INumberConverterService
    {
        string GenerateNumberWordsCompleteString(decimal number);
        
    }
}