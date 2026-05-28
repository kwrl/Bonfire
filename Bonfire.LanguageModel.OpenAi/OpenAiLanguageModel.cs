using OpenAI.Chat;
using Bonfire.Abstractions.LanguageModel;
using ChatMessage = Bonfire.Abstractions.LanguageModel.ChatMessage;

namespace Bonfire.LanguageModel.OpenAi;

/// <summary>
/// ILanguageModel implementation backed by an OpenAI-compatible API.
/// </summary>
public class OpenAiLanguageModel(ChatClient chatClient) : ILanguageModel
{
    public async Task<string> CompleteAsync(IEnumerable<ChatMessage> messages)
    {
        var chatMessages = messages.Select<ChatMessage, OpenAI.Chat.ChatMessage>(m => m.Role switch
        {
            Bonfire.Abstractions.LanguageModel.ChatMessageRole.System => new SystemChatMessage(m.Content),
            Bonfire.Abstractions.LanguageModel.ChatMessageRole.User => new UserChatMessage(m.Content),
            Bonfire.Abstractions.LanguageModel.ChatMessageRole.Assistant => new AssistantChatMessage(m.Content),
            _ => new UserChatMessage(m.Content)
        }).ToList();

        var completion = await chatClient.CompleteChatAsync(chatMessages);
        return completion.Value.Content[0].Text;
    }
}
