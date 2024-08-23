using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.Recipe.Register;

public class RecipeRegisterTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly Guid _userIdentifier;
    
    public RecipeRegisterTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.User.UserIdentifier;
    }

    [Fact]
    public async Task Success()
    {
        var request = RecipeRequestJsonBuilder.Build();

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await PostAsFormData(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().Should().NotBeNullOrWhiteSpace();
        responseData.RootElement.GetProperty("title").GetString().Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task EmptyTitleError(string culture)
    {
        var request = RecipeRequestJsonBuilder.Build();
        request.Title = string.Empty;

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await PostAsFormData(Endpoint, request, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        var msg = ResourceMessageExceptions.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));
        
        errors.Should().HaveCount(1).And.Contain(e => e.GetString()!.Equals(msg));
    }
}