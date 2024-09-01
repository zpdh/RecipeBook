using System.Text.Json;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.User.Profile;

public class GetUserProfileTest : RecipeBookClassFixture
{
    private const string Endpoint = "user";

    private readonly string _name;
    private readonly string _email;
    private readonly Guid _userIdentifier;

    public GetUserProfileTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _name = factory.User.Name;
        _email = factory.User.Email;
        _userIdentifier = factory.User.UserIdentifier;
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Get(Endpoint, token);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString()
            .Should().NotBeNullOrWhiteSpace().And.Be(_name);

        responseData.RootElement.GetProperty("email").GetString()
            .Should().NotBeNullOrWhiteSpace().And.Be(_email);
    }
}