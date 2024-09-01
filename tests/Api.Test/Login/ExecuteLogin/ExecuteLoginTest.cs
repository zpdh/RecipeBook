using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Api.Test.Login.ExecuteLogin;

public class ExecuteLoginTest : RecipeBookClassFixture
{
    private const string Endpoint = "login";

    private readonly RecipeBook.Domain.Entities.User _user;
    private readonly string _password;

    public ExecuteLoginTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _user = factory.User;
        _password = factory.Password;
    }

    [Fact]
    public async Task Success()
    {
        var request = new LoginRequestJson
        {
            Email = _user.Email,
            Password = _password
        };

        var response = await Post(Endpoint, request);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        responseData.RootElement.GetProperty("name").GetString()
            .Should().NotBeNullOrWhiteSpace().And.Be(_user.Name);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task InvalidLoginError(string culture)
    {
        var request = LoginRequestJsonBuilder.Build();
        
        var response = await Post(Endpoint, request, culture: culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var msg = ResourceMessageExceptions.ResourceManager.GetString(
            "EMAIL_OR_PASSWORD_INVALID",
            new CultureInfo(culture));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(msg));
    }
}