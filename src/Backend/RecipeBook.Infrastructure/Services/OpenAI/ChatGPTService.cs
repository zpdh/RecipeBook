using OpenAI_API;
using RecipeBook.Domain.DTOs;
using RecipeBook.Domain.Enums;
using RecipeBook.Domain.Extensions;
using RecipeBook.Domain.Services.OpenAI;

namespace RecipeBook.Infrastructure.Services.OpenAI;

public class ChatGPTService : IGenerateRecipeAI
{
    /*
     * Disclaimer: I do not have any OpenAI tokens.
     * Nothing related to AI has been tested, only coded.
     * (something is probably broken)
     */

    private const string ChatModel = "gpt-4o";
    private readonly IOpenAIAPI _openAIAPI;

    public ChatGPTService(IOpenAIAPI openAIAPI)
    {
        _openAIAPI = openAIAPI;
    }

    public async Task<GeneratedRecipeDto> Generate(IList<string> ingredients)
    {
        var conversation = _openAIAPI.Chat.CreateConversation( /*new ChatRequest { Model = ChatModel }*/);

        conversation.AppendSystemMessage(ResourceOpenAI.STARTING_MESSAGE);

        conversation.AppendUserInput(string.Join(';', ingredients));

        var response = await conversation.GetResponseFromChatbotAsync();

        var info = response
            .Split("\n")
            .Where(resp => string.IsNullOrWhiteSpace(resp).IsFalse())
            .Select(item => item.Replace("[", " ").Replace("]", " "))
            .ToList();

        var step = 1;

        return new GeneratedRecipeDto
        {
            Title = info[0],
            CookingTime = (CookingTime)Enum.Parse(typeof(CookingTime), info[1]),
            Ingredients = info[2].Split(";"),
            Instructions = info[3].Split("@").Select(instruction => new GeneratedInstructionDto
            {
                Text = instruction.Trim(),
                Step = step++
            }).ToList()
        };
    }
}