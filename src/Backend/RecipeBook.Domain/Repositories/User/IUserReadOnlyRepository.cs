namespace RecipeBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    public Task<bool> ActiveUserWithEmailExists(string email);
    public Task<bool> ActiveUserWithIdentifierExists(Guid userIdentifier);
    public Task<Entities.User?> GetByEmail(string email);
}