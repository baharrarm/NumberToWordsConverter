namespace NumberToWordsConverter.Api.Models;

public class ConvertNumberRequest
{
    // Accept string to accept numbers beyond JavaScript's exact numeric range.
    public string? Number { get; set; }
}

