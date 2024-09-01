using Azure.Messaging.ServiceBus;
using RecipeBook.Application.UseCases.User.Delete.Delete;
using RecipeBook.Infrastructure.Services.ServiceBus;

namespace RecipeBook.API.BackgroundServices;

public class DeleteUserService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ServiceBusProcessor _processor;

    public DeleteUserService(IServiceProvider services, DeleteUserProcessor processor)
    {
        _services = services;
        _processor = processor.Processor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        await _processor.StartProcessingAsync(stoppingToken);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var msg = args.Message.Body.ToString();

        var userIdentifier = Guid.Parse(msg);

        var scope = _services.CreateScope();

        var useCase = scope.ServiceProvider.GetRequiredService<IDeleteUserUseCase>();

        await useCase.Execute(userIdentifier);
    }

    private static Task ProcessErrorAsync(ProcessErrorEventArgs _)
    {
        // In the future, could make this method
        // either send an email or rerun the task
        // if it fails.

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        base.Dispose();

        GC.SuppressFinalize(this);
    }

    ~DeleteUserService() => Dispose();
}