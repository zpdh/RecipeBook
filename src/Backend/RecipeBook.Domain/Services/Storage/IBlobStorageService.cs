using RecipeBook.Domain.Entities;

namespace RecipeBook.Domain.Services.Storage;

public interface IBlobStorageService
{
    Task Upload(User user, Stream file, string imageIdentifier);
    Task<string> GetFileUrl(User user, string imageIdentifier);
    Task Delete(User user, string imageIdentifier);
    Task DeleteContainer(Guid userIdentifier);
}