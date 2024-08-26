using RecipeBook.Domain.Entities;

namespace RecipeBook.Domain.Services.ServiceBus;

public interface IDeleteUserQueue
{
    public Task SendMessage(User user);
}