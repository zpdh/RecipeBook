using System.Net;
using CommonTestUtils.Requests;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.Register;

public class RecipeRegisterInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";
    
    public RecipeRegisterInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
    }
    
    [Fact]
    public async Task InvalidTokenError()
    {
        var request = RecipeRequestJsonBuilder.Build();
        
        var response = await Post(Endpoint, request, "look at me!");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var request = RecipeRequestJsonBuilder.Build();

        var response = await Post(Endpoint, request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserNotFoundError()
    {
        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());

        var request = RecipeRequestJsonBuilder.Build();

        var response = await Post(Endpoint, request, token);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

}