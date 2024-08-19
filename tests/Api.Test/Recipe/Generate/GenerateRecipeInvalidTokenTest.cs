using System.Net;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.Generate;

public class GenerateRecipeInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe/generate";

    public GenerateRecipeInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var request = GenerateRecipeRequestJsonBuilder.Build();
        var response = await Post(Endpoint, request, "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var request = GenerateRecipeRequestJsonBuilder.Build();
        var response = await Post(Endpoint, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserNotFoundError()
    {
        var request = GenerateRecipeRequestJsonBuilder.Build();
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
        var response = await Post(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}