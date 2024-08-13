using System.Net;
using CommonTestUtils.Cryptography;
using CommonTestUtils.Tokens;
using FluentAssertions;

namespace Api.Test.Recipe.GetById;

public class GetRecipeByIdInvalidTokenTest : RecipeBookClassFixture
{
    private const string Endpoint = "recipe";
    private readonly long _id;

    public GetRecipeByIdInvalidTokenTest(CustomWebApplicationFactory factory) : base(factory)
    {
        _id = factory.Recipe.Id;
    }

    [Fact]
    public async Task InvalidTokenError()
    {
        var id = IdEncoderBuilder.Build().Encode(_id);

        var result = await Get($"{Endpoint}/{id}", "look at me!");

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task NoTokenError()
    {
        var id = IdEncoderBuilder.Build().Encode(_id);

        var result = await Get($"{Endpoint}/{id}");

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UnknownTokenError()
    {
        var id = IdEncoderBuilder.Build().Encode(_id);

        var token = JwtTokenGeneratorBuilder.Build().Generate(Guid.NewGuid());
        var result = await Get($"{Endpoint}/{id}", token);

        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}