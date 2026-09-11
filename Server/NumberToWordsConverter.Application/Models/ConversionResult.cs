namespace NumberToWordsConverter.Application.Models;

public record ConversionResult(
    bool IsSuccess,
    string Words,
    string Error
);