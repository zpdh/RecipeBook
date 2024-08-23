using Bogus;
using Moq;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Services.Storage;

namespace CommonTestUtils.Blob;

public class BlobStorageServiceBuilder
{
    private readonly Mock<IBlobStorageService> _mock;

    public BlobStorageServiceBuilder()
    {
        _mock = new Mock<IBlobStorageService>();
    }

    public IBlobStorageService Build()
    {
        return _mock.Object;
    }

    public BlobStorageServiceBuilder GetFileUrl(User user, string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return this;

        var faker = new Faker();
        var imageUrl = faker.Image.LoremFlickrUrl();

        _mock.Setup(blob => blob.GetFileUrl(user, fileName)).ReturnsAsync(imageUrl);

        return this;
    }

    public BlobStorageServiceBuilder GetFileUrl(User user, IList<Recipe> recipes)
    {
        var faker = new Faker();
        foreach (var recipe in recipes)
        {
            GetFileUrl(user, recipe.ImageIdentifier);
        }

        return this;
    }
}