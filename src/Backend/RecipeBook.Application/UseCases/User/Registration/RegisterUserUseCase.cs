using AutoMapper;
using FluentValidation.Results;
using RecipeBook.Application.Services.Cryptography;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.User.Registration;

public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IUserWriteOnlyRepository _writeOnlyRepository;
    private readonly IUserReadOnlyRepository _readOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly PasswordEncrypter _encrypter;

    public RegisterUserUseCase(
        IUserWriteOnlyRepository writeOnlyRepository,
        IUserReadOnlyRepository readOnlyRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        PasswordEncrypter encrypter)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _readOnlyRepository = readOnlyRepository;
        _mapper = mapper;
        _encrypter = encrypter;
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterUserResponseJson> Execute(RegisterUserRequestJson request)
    {
        await Validate(request);

        var user = _mapper.Map<Domain.Entities.User>(request);
        user.Password = _encrypter.Encrypt(request.Password);

        await _writeOnlyRepository.Add(user);
        await _unitOfWork.Commit();

        return new RegisterUserResponseJson
        {
            Name = request.Name
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

        if (!result.IsValid)
        {
            var errorMessages = result.Errors.Select(e => e.ErrorMessage).ToList();

            throw new ErrorOnValidationException(errorMessages);
        }
    }
}