using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using RecipeBook.Application.UseCases.Recipe.Register;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace UseCases.Tests.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task Success()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RecipeRequestJsonBuilder.Build();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }

    [Fact]
    public async Task EmptyTitleError()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RecipeRequestJsonBuilder.Build();
        request.Title = string.Empty;

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.ErrorMessages.Count == 1
                        && e.ErrorMessages.Contains(ResourceMessageExceptions.RECIPE_TITLE_EMPTY));
    }
    
    private static IRegisterRecipeUseCase CreateUseCase(RecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var writeRepo = RecipeWriteOnlyRepositoryBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);

        return new RegisterRecipeUseCase(writeRepo, unitOfWork, mapper, loggedUser);
    }
}