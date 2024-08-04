using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.User.Register;

public class UserRegisterTest : RecipeBookClassFixture
{
    private const string Endpoint = "user";

    public UserRegisterTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Success()
    {
        var request = RegisterUserRequestJsonBuilder.Build();

        var response = await Post(Endpoint, request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("name").GetString()
            .Should().NotBeNullOrWhiteSpace().And.Be(request.Name);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task EmptyNameError(string culture)
    {
        var request = RegisterUserRequestJsonBuilder.Build();
        request.Name = string.Empty;
        
        var response = await Post(Endpoint, request, culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var msg = ResourceMessageExceptions.ResourceManager.GetString("NAME_EMPTY", new CultureInfo(culture));

        errors.Should().ContainSingle().And.Contain(e => e.GetString()!.Equals(msg));
    }
}