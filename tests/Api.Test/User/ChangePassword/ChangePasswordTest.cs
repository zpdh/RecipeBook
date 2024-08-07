using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Api.Test.User.ChangePassword;

public class ChangePasswordTest : RecipeBookClassFixture
{
    private const string Endpoint = "user/change-password";

    private readonly string _email;
    private readonly string _password;
    private readonly Guid _userIdentifier;

    public ChangePasswordTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _email = factory.User.Email;
        _password = factory.Password;
        _userIdentifier = factory.User.UserIdentifier;
    }

    [Fact]
    public async Task Success()
    {
        var request = ChangePasswordRequestJsonBuilder.Build();
        request.Password = _password;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Put(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var login = new LoginRequestJson
        {
            Email = _email,
            Password = _password
        };

        response = await Post("login", login);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        login.Password = request.NewPassword;

        response = await Post("login", login);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task NewPasswordEmptyError(string culture)
    {
        var request = new ChangePasswordRequestJson
        {
            Password = _password,
            NewPassword = string.Empty
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Put(Endpoint, request, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var msg = ResourceMessageExceptions.ResourceManager.GetString("PASSWORD_INVALID", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.ContainSingle(e => e.GetString()!.Equals(msg));
    }
}