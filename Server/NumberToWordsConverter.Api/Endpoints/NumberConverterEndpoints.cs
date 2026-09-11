using NumberToWordsConverter.Api.Models;
using NumberToWordsConverter.Application.Services.Interfaces;

namespace NumberToWordsConverter.Api.Endpoints;

public static class NumberConverterEndpoints
{
    public static IEndpointRouteBuilder MapNumberConverterEndpoints(this IEndpointRouteBuilder app)
    {

        app.MapPost("/api/convert", (ConvertNumberRequest model, INumberConverterService numberConverterService) =>
        {
            if (!model.Number.HasValue)
                return Results.BadRequest(new {error = "Number is required."});

            var result = numberConverterService.GenerateNumberWordsCompleteString(model.Number.Value);
            if (!result.IsSuccess)
            {
                return Results.BadRequest(new {error = result.Error});
            }
            return Results.Ok(result.Words);
        });
        
        return app;
    }
}
