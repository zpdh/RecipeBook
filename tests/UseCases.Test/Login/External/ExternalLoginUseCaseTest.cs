using CommonTestUtils.Entities;
using CommonTestUtils.Repositories;
using CommonTestUtils.Tokens;
using FluentAssertions;
using RecipeBook.Application.UseCases.Login.ExternalLogin;

namespace UseCases.Tests.Login.External;

public class ExternalLoginUseCaseTest
{
    [Fact]
    public async Task AccountExistsSuccess()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);

        var result = await useCase.Execute(user.Name, user.Password);

        result.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task AccountCreationSuccess()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase();

        var result = await useCase.Execute(user.Name, user.Password);

        result.Should().NotBeNullOrWhiteSpace();
    }

    // No error methods for this use case test

    private static ExternalLoginUseCase CreateUseCase(RecipeBook.Domain.Entities.User? user = null)
    {
        var tokenGenerator = JwtTokenGeneratorBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var readRepo = new UserReadOnlyRepositoryBuilder();
        var writeRepo = UserWriteOnlyRepositoryBuilder.Build();

        if (user is not null)
        {
            readRepo.GetByEmail(user);
        }

        return new ExternalLoginUseCase(readRepo.Build(), writeRepo, unitOfWork, tokenGenerator);
    }
}