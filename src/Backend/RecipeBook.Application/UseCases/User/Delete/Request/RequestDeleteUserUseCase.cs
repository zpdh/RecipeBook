using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.ServiceBus;

namespace RecipeBook.Application.UseCases.User.Delete.Request;

public class RequestDeleteUserUseCase : IRequestDeleteUserUseCase
{
    private readonly IUserUpdateOnlyRepository _updateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly IDeleteUserQueue _queue;


    public RequestDeleteUserUseCase(
        IUserUpdateOnlyRepository updateOnlyRepository,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser,
        IDeleteUserQueue queue)
    {
        _updateOnlyRepository = updateOnlyRepository;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _queue = queue;
    }

    public async Task Execute()
    {
        var loggedUser = await _loggedUser.User();

        var user = await _updateOnlyRepository.GetById(loggedUser.Id);
        user.IsActive = false;

        _updateOnlyRepository.Update(user);
        await _unitOfWork.Commit();

        await _queue.SendMessage(user);
    }
}