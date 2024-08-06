using System.Net;
using CommonTestUtils.Tokens;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Api.Test.User.Profile;

public class GetUserProfileInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "user";

    public GetUserProfileInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var response = await Get(Endpoint, "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var response = await Get(Endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserNotFoundError()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await Get(Endpoint, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}