using Azure.Messaging.ServiceBus;

namespace RecipeBook.Infrastructure.Services.ServiceBus;

public class DeleteUserProcessor
{
    public ServiceBusProcessor Processor { get; }

    public DeleteUserProcessor(ServiceBusProcessor processor)
    {
        Processor = processor;
    }
}