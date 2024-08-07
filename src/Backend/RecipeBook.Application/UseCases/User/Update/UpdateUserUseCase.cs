using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.User.Update;

public class UpdateUserUseCase : IUpdateUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _updateOnlyRepository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;


    public UpdateUserUseCase(
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository updateOnlyRepository,
        IUserReadOnlyRepository readOnlyRepository,
        IUnitOfWork unitOfWork)
    {
        _loggedUser = loggedUser;
        _updateOnlyRepository = updateOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(UpdateUserRequestJson request)
    {
        var loggedUser = await _loggedUser.User();

        await Validate(request, loggedUser.Email);

        var user = await _updateOnlyRepository.GetById(loggedUser.Id);

        user.Name = request.Name;
        user.Email = request.Email;

        _updateOnlyRepository.Update(user);

        await _unitOfWork.Commit();
    }

    private async Task Validate(UpdateUserRequestJson request, string userEmail)
    {
        var validator = new UpdateUserValidator();

        var result = await validator.ValidateAsync(request);

        if (userEmail != request.Email)
        {
            var userExists = await _readOnlyRepository.ActiveUserWithEmailExists(request.Email);

            if (userExists)
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                    "email",
                    ResourceMessageExceptions.EMAIL_ALREADY_REGISTERED));
            }
        }

        if (result.IsValid) return;

        var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

        throw new ErrorOnValidationException(errorMessages);
    }
}