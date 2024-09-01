using CommonTestUtils.Blob;
using CommonTestUtils.Entities;
using CommonTestUtils.LoggedUser;
using CommonTestUtils.Mapper;
using CommonTestUtils.Repositories;
using CommonTestUtils.Requests;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RecipeBook.Application.UseCases.Recipe.Register;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;
using UseCases.Tests.Recipe.InlineData;

namespace UseCases.Tests.Recipe.Register;

public class RegisterRecipeUseCaseTest
{
    [Fact]
    public async Task SuccessWithoutImage()
    {
        var (user, _) = UserBuilder.Build();

        var useCase = CreateUseCase(user);

        var request = RegisterRecipeFormDataRequestBuilder.Build();

        var result = await useCase.Execute(request);

        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrWhiteSpace();
        result.Title.Should().Be(request.Title);
    }

    [Theory]
    [ClassData(typeof(ImageTypesInlineData))]
    public async Task SuccessWithImage(IFormFile file)
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);
        var request = RegisterRecipeFormDataRequestBuilder.Build(file);

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

        var request = RegisterRecipeFormDataRequestBuilder.Build();
        request.Title = string.Empty;

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.RECIPE_TITLE_EMPTY));
    }

    [Fact]
    public async Task InvalidFileError()
    {
        var (user, _) = UserBuilder.Build();
        var useCase = CreateUseCase(user);
        var file = FormFileBuilder.Txt();
        var request = RegisterRecipeFormDataRequestBuilder.Build(file);

        var act = async () => await useCase.Execute(request);

        (await act.Should().ThrowAsync<ErrorOnValidationException>())
            .Where(e => e.GetErrorMessages().Count == 1
                        && e.GetErrorMessages().Contains(ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED));
    }

    private static RegisterRecipeUseCase CreateUseCase(RecipeBook.Domain.Entities.User user)
    {
        var mapper = MapperBuilder.Build();
        var unitOfWork = UnitOfWorkBuilder.Build();
        var writeRepo = RecipeWriteOnlyRepositoryBuilder.Build();
        var loggedUser = LoggedUserBuilder.Build(user);
        var blobStorage = new BlobStorageServiceBuilder().Build();

        return new RegisterRecipeUseCase(writeRepo, unitOfWork, mapper, loggedUser, blobStorage);
    }
}