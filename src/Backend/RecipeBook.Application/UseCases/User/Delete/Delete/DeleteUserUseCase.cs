using RecipeBook.Domain.Repositories;
using RecipeBook.Domain.Repositories.User;
using RecipeBook.Domain.Services.Storage;

namespace RecipeBook.Application.UseCases.User.Delete.Delete;

public class DeleteUserUseCase : IDeleteUserUseCase
{
    private readonly IUserDeleteOnlyRepository _deleteOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobStorageService _blobStorageService;

    public DeleteUserUseCase(
        IUserDeleteOnlyRepository deleteOnlyRepository,
        IUnitOfWork unitOfWork,
        IBlobStorageService blobStorageService)
    {
        _deleteOnlyRepository = deleteOnlyRepository;
        _unitOfWork = unitOfWork;
        _blobStorageService = blobStorageService;
    }

    public async Task Execute(Guid userIdentifier)
    {
        await _blobStorageService.DeleteContainer(userIdentifier);
        await _deleteOnlyRepository.Delete(userIdentifier);

        await _unitOfWork.Commit();
    }
}