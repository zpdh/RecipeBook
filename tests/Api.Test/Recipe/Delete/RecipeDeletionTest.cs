using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.Recipe.Delete;

public class RecipeDeletionTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly Guid _userIdentifier;
    private readonly string _recipeId;

    public RecipeDeletionTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _userIdentifier = factory.User.UserIdentifier;
        _recipeId = encoder.Encode(factory.Recipe.Id);
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Delete($"{Endpoint}/{_recipeId}", token);
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        response = await Get($"{Endpoint}/{_recipeId}", token);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task RecipeNotFound(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
        var id = IdEncoderBuilder.Build().Encode(999);

        var response = await Delete($"{Endpoint}/{id}", token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        var msg = ResourceMessageExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

        errors.Should().HaveCount(1)
            .And.Contain(e => e.GetString()!.Equals(msg));
    }
}