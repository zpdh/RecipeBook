using System.Net;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Dashboard.Get;

public class GetDashboardInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "dashboard";

    public GetDashboardInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task EmptyTokenError()
    {
        var response = await Get(Endpoint);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var response = await Get(Endpoint, "look at me!");

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