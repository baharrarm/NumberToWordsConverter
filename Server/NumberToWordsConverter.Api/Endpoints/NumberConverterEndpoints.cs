using System.Globalization;
using System.Text.RegularExpressions;
using NumberToWordsConverter.Api.Models;
using NumberToWordsConverter.Application.Services.Interfaces;

namespace NumberToWordsConverter.Api.Endpoints;

public static class NumberConverterEndpoints
{
    // Allow decimal notation, reject scientific notation and allow trailing zeros.
    // Reject grouping separators and nonzero digits after 2-digit cents.
    private static readonly Regex NumberRegex = new(@"\A-?(?:[0-9]+(?:\.[0-9]{1,2}0*)?|\.[0-9]{1,2}0*)\z");
    public static IEndpointRouteBuilder MapNumberConverterEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/api/convert", (ConvertNumberRequest model, INumberConverterService numberConverterService) =>
        {
            if (string.IsNullOrWhiteSpace(model.Number))
                return Results.BadRequest(new {error = "Number is required."});
            
            var input = model.Number.Trim();
            if (!NumberRegex.IsMatch(input))
                return Results.BadRequest(new {error = "Enter a valid number with up to two decimal places."});

            var allowed = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint;
            if (!decimal.TryParse(input, allowed, CultureInfo.InvariantCulture, out var number))
                return Results.BadRequest(new {error = "The number is too big."});

            var result = numberConverterService.GenerateNumberWordsCompleteString(number);

            if (!result.IsSuccess)
                return Results.BadRequest(new {error = result.Error});

            return Results.Ok(result.Words);
        });
        
        return app;
    }
}
