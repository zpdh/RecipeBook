using System.Net;
using System.Text.Json;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Dashboard.Get;

public class GetDashboardTest : RecipeBookClassFixture
{
    private const string Endpoint = "dashboard";

    private readonly Guid _userIdentifier;

    public GetDashboardTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.User.UserIdentifier;
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Get(Endpoint, token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").GetArrayLength().Should().BeGreaterThan(0);
    }
}