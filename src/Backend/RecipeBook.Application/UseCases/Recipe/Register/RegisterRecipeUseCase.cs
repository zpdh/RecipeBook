using AutoMapper;
using FileTypeChecker.Extensions;
using FileTypeChecker.Types;
using RecipeBook.Application.Extensions;
using RecipeBook.Communication.Requests;
using RecipeBook.Communication.Responses;
using RecipeBook.Domain.Entities;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.Recipe;
using RecipeBook.Domain.Services.LoggedUser;
using RecipeBook.Domain.Services.Storage;
using RecipeBook.Exceptions;
using RecipeBook.Exceptions.ExceptionsBase;

namespace RecipeBook.Application.UseCases.Recipe.Register;

public class RegisterRecipeUseCase : IRegisterRecipeUseCase
{
    private readonly IRecipeWriteOnlyRepository _writeOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILoggedUser _loggedUser;
    private readonly IBlobStorageService _blobStorageService;

    public RegisterRecipeUseCase(
        IRecipeWriteOnlyRepository writeOnlyRepository,
        IUnitOfWork unitOfWork, IMapper mapper,
        ILoggedUser loggedUser,
        IBlobStorageService blobStorageService)
    {
        _writeOnlyRepository = writeOnlyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _loggedUser = loggedUser;
        _blobStorageService = blobStorageService;
    }

    public async Task<RegisteredRecipeResponseJson> Execute(RegisterRecipeFormDataRequest request)
    {
        Validate(request);

        var user = await _loggedUser.User();

        var recipe = _mapper.Map<Domain.Entities.Recipe>(request);
        recipe.UserId = user.Id;

        var instructions = request.Instructions.OrderBy(instruction => instruction.Step).ToList();

        for (var i = 0; i < instructions.Count; i++)
        {
            instructions[i].Step = i + 1;
        }

        recipe.Instructions = _mapper.Map<IList<Instruction>>(instructions);

        if (request.Image is not null)
        {
            var fileStream = request.Image.OpenReadStream();
            var (isStreamValid, extension) = fileStream.ValidateAndGetImageExtension();

            if (isStreamValid.IsFalse())
            {
                throw new ErrorOnValidationException([ResourceMessageExceptions.ONLY_IMAGES_ACCEPTED]);
            }

            recipe.ImageIdentifier = $"{Guid.NewGuid()}{extension}";

            await _blobStorageService.Upload(user, fileStream, recipe.ImageIdentifier);
        }

        await _writeOnlyRepository.Add(recipe);

        await _unitOfWork.Commit();

        var response = _mapper.Map<RegisteredRecipeResponseJson>(recipe);
        return response;
    }

    private static void Validate(RecipeRequestJson request)
    {
        var validator = new RecipeValidator();

        var result = validator.Validate(request);

        if (result.IsValid.IsFalse())
        {
            throw new ErrorOnValidationException(result.Errors.Select(e => e.ErrorMessage).Distinct().ToList());
        }
    }
}