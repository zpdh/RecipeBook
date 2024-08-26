namespace RecipeBook.Domain.Repositories.User;

public interface IUserDeleteOnlyRepository
{
    Task Delete(Guid userIdentifier);
}