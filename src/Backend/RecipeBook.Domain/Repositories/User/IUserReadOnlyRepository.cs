namespace RecipeBook.Domain.Repositories.User;

public interface IUserReadOnlyRepository
{
    public Task<bool> ActiveUserWithEmailExists(string email);

    public Task<Entities.User?> GetByEmailAndPassword(string email, string password);
}