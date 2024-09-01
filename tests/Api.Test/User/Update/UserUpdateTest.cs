using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.User.Update;

public class UserUpdateTest : RecipeBookClassFixture
{
    private const string Endpoint = "user";
    private readonly Guid _userIdentifier;

    public UserUpdateTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.User.UserIdentifier;
    }

    [Fact]
    public async Task Success()
    {
        var request = UpdateUserRequestJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Put(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task EmptyNameError(string culture)
    {
        var request = UpdateUserRequestJsonBuilder.Build();
        request.Name = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Put(Endpoint, request, token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var expectedMsg = ResourceMessageExceptions.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.ContainSingle(error => error.GetString()!.Equals(expectedMsg));
    }
}