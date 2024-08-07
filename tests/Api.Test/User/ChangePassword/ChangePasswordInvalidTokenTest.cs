using System.Net;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Communication.Requests;

namespace Api.Test.User.ChangePassword;

public class ChangePasswordInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "user/change-password";

    public ChangePasswordInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]

    public async Task InvalidTokenError()
    {
        var request = new ChangePasswordRequestJson();

        var response = await Put(Endpoint, request, "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var request = new ChangePasswordRequestJson();

        var response = await Put(Endpoint, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task TokenWithUserNotFoundError()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var request = new ChangePasswordRequestJson();

        var response = await Put(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}