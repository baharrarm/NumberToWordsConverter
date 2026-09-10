using NumberToWordsConverter.Api.Models;
using NumberToWordsConverter.Application.Services.Interfaces;
using NumberToWordsConverter.Application.Validators;

namespace NumberToWordsConverter.Api.Endpoints;

public static class NumberConverterEndpoints
{
    public static IEndpointRouteBuilder MapNumberConverterEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/api/convert", (ConvertNumberRequest model, NumberValidator validator, INumberConverterService numberConverterService) =>
        {
            if (!model.Number.HasValue)
                return Results.BadRequest(new {error = "Number is required."});

            var number = model.Number.Value;
            var error = validator.Validate(number);
            if (!string.IsNullOrEmpty(error)) 
                return Results.BadRequest(new {error});

            var result = numberConverterService.GenerateNumberWordsCompleteString(number);
            return Results.Ok(result);
        });
        
        return app;
    }
}
