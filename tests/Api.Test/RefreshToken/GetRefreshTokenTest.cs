using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using FluentAssertions;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Api.Test.RefreshToken;

public class GetRefreshTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "token";

    private readonly string _refreshToken;

    public GetRefreshTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _refreshToken = factory.RefreshToken.Value;
    }

    [Fact]
    public async Task Success()
    {
        var request = new NewTokenRequestJson
        {
            RefreshToken = _refreshToken
        };

        var response = await Post($"{Endpoint}/refresh-token", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("accessToken").GetString().Should().NotBeNull();
        responseData.RootElement.GetProperty("refreshToken").GetString().Should().NotBeNull();
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task TokenNotFoundError(string culture)
    {
        var request = new NewTokenRequestJson
        {
            RefreshToken = "look at me!"
        };

        var response = await Post($"{Endpoint}/refresh-token", request, culture: culture);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var msg = ResourceMessageExceptions.ResourceManager.GetString("REFRESH_TOKEN_NOT_FOUND",
            new CultureInfo(culture));

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.Should().ContainSingle()
            .And.Contain(e => e.GetString()!.Equals(msg));
    }
}