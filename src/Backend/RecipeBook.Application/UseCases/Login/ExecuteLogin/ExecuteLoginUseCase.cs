using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Cryptography;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Login.ExecuteLogin;

public class ExecuteLoginUseCase : IExecuteLoginUseCase
{
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IPasswordEncrypter _encrypter;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public ExecuteLoginUseCase(
        IUserReadOnlyRepository readOnlyRepository,
        IPasswordEncrypter encrypter,
        IAccessTokenGenerator tokenGenerator)
    {
        _readOnlyRepository = readOnlyRepository;
        _encrypter = encrypter;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<RegisterUserResponseJson> Execute(LoginRequestJson request)
    {
        /*
         * Why is there no validation?
         * There's no need for validation, since the database shouldn't have any invalid requests.
         * Validation would be justified if there was a user base for the app,
         * that way it would result in a performance increase.
         */

        var user = await _readOnlyRepository.GetByEmail(request.Email);

        // Checks if user exists and if given password
        // is equal to password in database.
        if (user is null || _encrypter.IsValid(request.Password, user.Password).IsFalse())
        {
            throw new InvalidLoginException();
        }

        return new RegisterUserResponseJson
        {
            Name = user.Name,
            Tokens = new ResponseTokenJson
            {
                AccessToken = _tokenGenerator.Generate(user.UserIdentifier)
            }
        };
    }
}