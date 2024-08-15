using System.Net;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.Update;

public class RecipeUpdateInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";

    private readonly string _recipeId;

    public RecipeUpdateInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        var encoder = IdEncoderBuilder.Build();

        _recipeId = encoder.Encode(factory.Recipe.Id);
    }

    [Fact]
    public async Task EmptyTokenError()
    {
        var request = RecipeRequestJsonBuilder.Build();

        var response = await Put($"{Endpoint}/{_recipeId}", request, "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var request = RecipeRequestJsonBuilder.Build();

        var response = await Put($"{Endpoint}/{_recipeId}", request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserNotFoundError()
    {
        var request = RecipeRequestJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var response = await Put($"{Endpoint}/{_recipeId}", request, token);
    }
}