using CommonTestUtils.Cryptography;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.User.Registration;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.User.Registration;

public class RegisterUserUseCaseTest
{

    [Fact]
    public async Task Success()
    {
        var useCase = InstanceUseCase();

        var request = RegisterUserRequestJsonBuilder.Build();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task EmailAlreadyRegisteredError()
    {
        var request = RegisterUserRequestJsonBuilder.Build();

        var useCase = InstanceUseCase(request.Email);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e =>
                e.ErrorMessages.Count == 1 &&
                e.ErrorMessages.Contains(ResourceMessageExceptions.EMAIL_EXISTS));
    }
    
    
    [Fact]
    public async Task EmptyNameError()
    {
        /*
         * Here I'm only testing 1 validator method since they've
         * been tested on their own before. It's just a check to
         * make sure it's going from the use case to the validator.
         */
        
        var request = RegisterUserRequestJsonBuilder.Build();
        request.Name = string.Empty;

        var useCase = InstanceUseCase();

        Func<Task> act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e =>
                e.ErrorMessages.Count == 1 &&
                e.ErrorMessages.Contains(ResourceMessageExceptions.NAME_EMPTY));
    }

    private static RegisterUserUseCase InstanceUseCase(string? email = null)
    {
        var encrypter = PasswordEncrypterBuilder.Build();
        var mapper = MapperBuilder.Build();
        var uow = UnitOfWorkBuilder.Build();
        var writeRepo = UserWriteOnlyRepositoryBuilder.Build();
        var readRepoBuilder = new UserReadOnlyRepositoryBuilder();

        if (!string.IsNullOrWhiteSpace(email))
        {
            readRepoBuilder.ActiveUserWithEmailExists(email);
        }

        return new RegisterUserUseCase(writeRepo, readRepoBuilder.Build(), mapper, uow, encrypter);
    }
}