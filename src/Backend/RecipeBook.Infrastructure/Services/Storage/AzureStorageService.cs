using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Domain.ValueObjects;

namespace RecipeBook.Infrastructure.Services.Storage;

public class AzureStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task Upload(User user, Stream file, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(file, overwrite: true);
    }

    public async Task<string> GetFileUrl(User user, string fileName)
    {
        var containerName = user.UserIdentifier.ToString();
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());

        var exists = await containerClient.ExistsAsync();

        if (exists.Value.IsFalse())
        {
            return string.Empty;
        }

        var blobClient = containerClient.GetBlobClient(fileName);
        exists = await blobClient.ExistsAsync();

        if (exists.Value.IsFalse())
        {
            return string.Empty;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(RuleConstants.MaximumImageUrlLifetimeInMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task Delete(User user, string fileName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        var exists = await containerClient.ExistsAsync();

        if (exists.Value.IsFalse()) return;

        await containerClient.DeleteBlobIfExistsAsync(fileName);
    }

    public async Task DeleteContainer(Guid userIdentifier)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());
        await containerClient.DeleteIfExistsAsync();
    }
}