using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Tokens;

namespace RecipeBook.Application.UseCases.Login.ExternalLogin;

public class ExternalLoginUseCase : IExternalLoginUseCase
{
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IUserWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public ExternalLoginUseCase(
        IUserReadOnlyRepository readOnlyRepository,
        IUserWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator tokenGenerator)
    {
        _readOnlyRepository = readOnlyRepository;
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<string> Execute(string name, string email)
    {
        var user = await _readOnlyRepository.GetByEmail(email);

        // There are a few approaches to this.
        // When the person registers with said
        // external source, you could ask them 
        // for a password for the account, not
        // allow them to log in with a password
        // or simply don't let the person register
        // with an external source. I chose to not 
        // allow the user to log in with a password
        // if they register with external sources.
        if (user is null)
        {
            user = new Domain.Entities.User
            {
                Name = name,
                Email = email,
                Password = "-"
            };

            await _writeOnlyRepository.Add(user);
            await _unitOfWork.Commit();
        }

        return _tokenGenerator.Generate(user.UserIdentifier);
    }
}