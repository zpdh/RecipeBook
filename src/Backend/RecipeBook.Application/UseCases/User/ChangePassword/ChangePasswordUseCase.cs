using RecipeBook.Communication.Requests;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Cryptography;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.User.ChangePassword;

public class ChangePasswordUseCase : IChangePasswordUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _updateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordEncrypter _encrypter;


    public ChangePasswordUseCase(
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository updateOnlyRepository,
        IUnitOfWork unitOfWork,
        IPasswordEncrypter encrypter)
    {
        _loggedUser = loggedUser;
        _updateOnlyRepository = updateOnlyRepository;
        _unitOfWork = unitOfWork;
        _encrypter = encrypter;
    }

    public async Task Execute(ChangePasswordRequestJson request)
    {
        var loggedUser = await _loggedUser.User();

        Validate(loggedUser, request);

        var user = await _updateOnlyRepository.GetById(loggedUser.Id);

        user.Password = _encrypter.Encrypt(request.NewPassword);

        _updateOnlyRepository.Update(user);

        await _unitOfWork.Commit();
    }

    private void Validate(Domain.Entities.User user, ChangePasswordRequestJson request)
    {
        var result = new ChangePasswordValidator().Validate(request);

        if (_encrypter.IsValid(request.Password, user.Password).IsFalse())
        {
            result.Errors.Add(new FluentValidation.Results.ValidationFailure(
                string.Empty,
                ResourceMessageExceptions.PASSWORD_INVALID));
        }

        if (result.IsValid) return;

        var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

        throw new ErrorOnValidationException(errorMessages);
    }
}