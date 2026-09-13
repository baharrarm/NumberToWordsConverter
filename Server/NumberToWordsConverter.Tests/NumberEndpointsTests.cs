using System.Net;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NumberToWordsConverter.Tests;

public class NumberConverterEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private record ErrorResponse(string Error);

    public NumberConverterEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
    }

    [Theory]
    [InlineData("{}", "Number is required.")]
    [InlineData("{\"number\": null}", "Number is required.")]
    public async Task ReturnsBadRequest_WhenNumberIsMissingOrNull(string json, string expected)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(body);
        Assert.Equal(expected, body.Error);
    }

    
    [Theory]
    [InlineData("{")]
    [InlineData("{\"number\": \"\"}")]
    [InlineData("{\"number\": \"   \"}")]
    [InlineData("{\"number\": \"abc\"}")]
    [InlineData("{\"number\": \"1,234.56\"}")]
    [InlineData("{\"number\": true}")]
    [InlineData("{\"number\": 23.}")]
    [InlineData("{\"number\": 79228162514264337593543950336}")] // exceeds C# decimal’s range
    public async Task ReturnsBadRequest_WhenJsonOrPropertyTypeIsInvalid(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    

    [Theory]
    [InlineData("{\"number\": 0}", "The number must be greater than zero.")]
    [InlineData("{\"number\": -1}", "The number must be greater than zero.")]
    [InlineData("{\"number\": 18446744073709551616}", "The number is too big.")]
    [InlineData("{\"number\": 1.234}", "The fraction must not be more than 2 digits.")]
    public async Task ReturnsBadRequest_WhenAmountIsInvalid(string json,string expected)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(body);
        Assert.Equal(expected, body.Error);
    }

    [Theory]
    [InlineData("{\"number\": 123.45}",
        "ONE HUNDRED AND TWENTY-THREE DOLLARS AND FORTY-FIVE CENTS")]
    [InlineData("{\"number\": 0.25}", "TWENTY-FIVE CENTS")]
    [InlineData("{\"number\": 1.2300}", "ONE DOLLAR AND TWENTY-THREE CENTS")]
    [InlineData("{\"number\": 18446744073709551615.99}",
        "EIGHTEEN QUINTILLION FOUR HUNDRED AND FORTY-SIX QUADRILLION " +
        "SEVEN HUNDRED AND FORTY-FOUR TRILLION SEVENTY-THREE BILLION " +
        "SEVEN HUNDRED AND NINE MILLION FIVE HUNDRED AND FIFTY-ONE THOUSAND " +
        "SIX HUNDRED AND FIFTEEN DOLLARS AND NINETY-NINE CENTS")]
    public async Task ReturnsWords_WhenNumberIsValid(string json, string expected)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var words = await response.Content.ReadFromJsonAsync<string>();
        Assert.Equal(expected, words);
    }
}