using RecipeBook.Application.Services.Cryptography;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Login.ExecuteLogin;

public class ExecuteLoginUseCase : IExecuteLoginUseCase
{
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly PasswordEncrypter _encrypter;

    public ExecuteLoginUseCase(IUserReadOnlyRepository readOnlyRepository, PasswordEncrypter encrypter)
    {
        _readOnlyRepository = readOnlyRepository;
        _encrypter = encrypter;
    }

    public async Task<RegisterUserResponseJson> Execute(LoginRequestJson request)
    {
        /*
         * Why is there no validation?
         * There's no need for validation, since the database shouldn't have any invalid requests.
         * Validation would be justified if there was a user base for the app,
         * that way it would result in a performance increase.
         */

        var encryptedPassword = _encrypter.Encrypt(request.Password);

        var user = await _readOnlyRepository.GetByEmailAndPassword(request.Email, encryptedPassword)
                   ?? throw new InvalidLoginException();

        return new RegisterUserResponseJson
        {
            Name = user.Name
        };
    }
}