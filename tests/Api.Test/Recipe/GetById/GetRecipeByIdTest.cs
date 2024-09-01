using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.Recipe.GetById;

public class GetRecipeByIdTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly Guid _userIdentifier;
    private readonly string _recipeId;
    private readonly string _recipeTitle;

    public GetRecipeByIdTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _userIdentifier = factory.User.UserIdentifier;
        _recipeId = encoder.Encode(factory.Recipe.Id);
        _recipeTitle = factory.Recipe.Title;
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Get($"{Endpoint}/{_recipeId}", token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("id").GetString().Should().Be(_recipeId);
        responseData.RootElement.GetProperty("title").GetString().Should().Be(_recipeTitle);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task RecipeNotFoundError(string culture)
    {
        var id = IdEncoderBuilder.Build().Encode(999);
        
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Get($"{Endpoint}/{id}", token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var msg = ResourceMessageExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();

        errors.Should().HaveCount(1)
            .And.Contain(e => e.GetString()!.Equals(msg));
    }
}