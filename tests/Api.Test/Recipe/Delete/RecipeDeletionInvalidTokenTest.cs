using System.Net;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.Delete;

public class RecipeDeletionInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly string _recipeId;

    public RecipeDeletionInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _recipeId = encoder.Encode(factory.Recipe.Id);
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var response = await Delete($"{Endpoint}/{_recipeId}", "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var response = await Delete($"{Endpoint}/{_recipeId}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnknownTokenError()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await Delete($"{Endpoint}/{_recipeId}", token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}