using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authentication.BearerToken;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Security.Cryptography;
using RecipeBook.Domain.Security.Tokens;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.User.Registration;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserWriteOnlyRepository _writeOnlyRepository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordEncrypter _encrypter;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public RegisterUserUseCase(
        IUserWriteOnlyRepository writeOnlyRepository,
        IUserReadOnlyRepository readOnlyRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IPasswordEncrypter encrypter,
        IAccessTokenGenerator tokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator,
        ITokenRepository tokenRepository)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _mapper = mapper;
        _encrypter = encrypter;
        _tokenGenerator = tokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _tokenRepository = tokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterUserResponseJson> Execute(RegisterUserRequestJson request)
    {
        await Validate(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _encrypter.Encrypt(request.Password);
        user.UserIdentifier = Guid.NewGuid();

        await _writeOnlyRepository.Add(user);
        await _unitOfWork.Commit();

        var refreshToken = await CreateRefreshToken(user);

        return new RegisterUserResponseJson
        {
            Name = user.Name,
            Tokens = new TokensResponseJson
            {
                AccessToken = _tokenGenerator.Generate(user.UserIdentifier),
                RefreshToken = refreshToken
            }
        };
    }

    private async Task Validate(RegisterUserRequestJson request)
    {
        var validator = new RegisterUserValidator();

        var result = await validator.ValidateAsync(request);
        var emailExists = await _readOnlyRepository.ActiveUserWithEmailExists(request.Email);

        if (emailExists)
        {
            result.Errors.Add(new ValidationFailure(string.Empty, ResourceMessageExceptions.EMAIL_EXISTS));
        }

        if (result.IsValid.IsFalse())
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }

    private async Task<string> CreateRefreshToken(Domain.Entities.User user)
    {
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Value = _refreshTokenGenerator.Generate(),
            UserId = user.Id
        };

        await _tokenRepository.SaveRefreshToken(refreshToken);
        await _unitOfWork.Commit();

        return refreshToken.Value;
    }
}