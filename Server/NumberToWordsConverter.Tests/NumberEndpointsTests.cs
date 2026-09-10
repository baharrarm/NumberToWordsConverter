using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;

namespace NumberToWordsConverter.Tests;

public class NumberConverterEndpointTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public NumberConverterEndpointTests(
        WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost"),
                AllowAutoRedirect = false
            });
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"number\": null}")]
    public async Task ReturnsBadRequest_WhenNumberIsMissingOrNull(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("{\"number\": \"abc\"}")]
    public async Task ReturnsBadRequest_WhenInvalidJsonOrNumber(string json)
    {
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("/api/convert", content);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}