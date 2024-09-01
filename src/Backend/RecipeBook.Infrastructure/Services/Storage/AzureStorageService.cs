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

    // NOTE: the 'imageIdentifier' parameter is
    // the file name, which is the image GUID 
    // for ease of access.

    public async Task Upload(User user, Stream file, string imageIdentifier)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(imageIdentifier);
        await blobClient.UploadAsync(file, overwrite: true);
    }

    public async Task<string> GetFileUrl(User user, string imageIdentifier)
    {
        var containerName = user.UserIdentifier.ToString();
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());

        var exists = await containerClient.ExistsAsync();

        if (exists.Value.IsFalse())
        {
            return string.Empty;
        }

        var blobClient = containerClient.GetBlobClient(imageIdentifier);
        exists = await blobClient.ExistsAsync();

        if (exists.Value.IsFalse())
        {
            return string.Empty;
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = imageIdentifier,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(RuleConstants.MaximumImageUrlLifetimeInMinutes)
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        return blobClient.GenerateSasUri(sasBuilder).ToString();
    }

    public async Task Delete(User user, string imageIdentifier)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(user.UserIdentifier.ToString());
        var exists = await containerClient.ExistsAsync();

        if (exists.Value.IsFalse()) return;

        await containerClient.DeleteBlobIfExistsAsync(imageIdentifier);
    }

    public async Task DeleteContainer(Guid userIdentifier)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(userIdentifier.ToString());
        await containerClient.DeleteIfExistsAsync();
    }
}