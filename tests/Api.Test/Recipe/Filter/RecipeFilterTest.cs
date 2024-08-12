using System.Globalization;
using System.Net;
using System.Text.Json;
using Api.Test.InlineData;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using RecipeBook.Communication.Enums;
using RecipeBook.Communication.Requests;
using RecipeBook.Exceptions;

namespace Api.Test.Recipe.Filter;

public class RecipeFilterTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe/filter";
    private readonly Guid _userIdentifier;
    private readonly RecipeBook.Domain.Entities.Recipe _recipe;

    public RecipeFilterTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _userIdentifier = factory.User.UserIdentifier;
        _recipe = factory.Recipe;
    }

    [Fact]
    public async Task Success()
    {
        var request = new RecipeFilterRequestJson
        {
            CookingTimes = [(CookingTime)_recipe.CookingTime!],
            Difficulties = [(Difficulty)_recipe.Difficulty!],
            DishTypes = _recipe.DishTypes.Select(dt => (DishType)dt.Type).ToList()
        };

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Post(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        responseData.RootElement.GetProperty("recipes").EnumerateArray().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task NoContentSuccess()
    {
        var request = RecipeFilterRequestJsonBuilder.Build();
        request.RecipeTitleOrIngredient = "look at me! I'm a silly little filler text!";

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Post(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Theory]
    [ClassData(typeof(CultureInlineDataTests))]
    public async Task InvalidCookingTimeError(string culture)
    {
        var request = RecipeFilterRequestJsonBuilder.Build();
        request.CookingTimes.Add((CookingTime)999);

        var token = JwtTokenGeneratorBuilder.Build().Generate(_userIdentifier);

        var response = await Post(Endpoint, request, token, culture);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errorMessages").EnumerateArray();
        var msg = ResourceMessageExceptions.ResourceManager.GetString("NOT_ENUM", new CultureInfo(culture));

        errors.Should().HaveCount(1).And.Contain(e => e.GetString()!.Equals(msg));
    }
}