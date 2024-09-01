using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Exceptions;

namespace Api.Test.Recipe.Update;

public class RecipeUpdateTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly Guid _userIdentifier;
    private readonly string _recipeId;

    public RecipeUpdateTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _userIdentifier = factory.User.UserIdentifier;
        _recipeId = encoder.Encode(factory.Recipe.Id);
    }

    [Fact]
    public async Task Success()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
        var request = RecipeRequestJsonBuilder.Build();

        var response = await Put($"{Endpoint}/{_recipeId}", request, token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task NotFoundError(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
        var request = RecipeRequestJsonBuilder.Build();

        var id = IdEncoderBuilder.Build().Encode(999);

        var response = await Put($"{Endpoint}/{id}", request, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        var msg = ResourceMessageExceptions.ResourceManager.GetString("RECIPE_NOT_FOUND", new CultureInfo(culture));

        errors.Should().HaveCount(1)
            .And.Contain(e => e.GetString()!.Equals(msg));
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task EmptyTitleError(string culture)
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);
        var request = RecipeRequestJsonBuilder.Build();
        request.Title = string.Empty;

        var response = await Put($"{Endpoint}/{_recipeId}", request, token, culture);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        var msg = ResourceMessageExceptions.ResourceManager.GetString("RECIPE_TITLE_EMPTY", new CultureInfo(culture));

        errors.Should().HaveCount(1)
            .And.Contain(e => e.GetString()!.Equals(msg));
    }
}