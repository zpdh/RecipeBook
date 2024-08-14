using System.Net;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.GetById;

public class GetRecipeByIdInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";
    private readonly string _recipeId;

    public GetRecipeByIdInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _recipeId = encoder.Encode(factory.Recipe.Id);
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var result = await Get($"{Endpoint}/{_recipeId}", "look at me!");

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var result = await Get($"{Endpoint}/{_recipeId}");

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnknownTokenError()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
        var result = await Get($"{Endpoint}/{_recipeId}", token);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}