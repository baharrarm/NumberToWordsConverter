using NumberToWordsConverter.Api.Models;
using NumberToWordsConverter.Application.Services.Interfaces;

namespace NumberToWordsConverter.Api.Endpoints;

public static class NumberConverterEndpoints
{
    public static IEndpointRouteBuilder MapNumberConverterEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/api/convert", (ConvertNumberRequest model, INumberConverterService numberConverterService) =>
        {
            var amount = Convert.ToDecimal(model.Number);
            var result = numberConverterService.GenerateNumberWordsCompleteString(amount);
            return Results.Ok(result);
        });
        
        return app;
    }
}
