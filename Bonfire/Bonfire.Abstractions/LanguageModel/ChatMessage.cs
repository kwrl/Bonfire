namespace Bonfire.Abstractions.LanguageModel;

/// <summary>
/// Represents a message in a chat completion request.
/// </summary>
public record ChatMessage(ChatMessageRole Role, string Content);

/// <summary>
/// The role of a chat message.
/// </summary>
public enum ChatMessageRole
{
    System,
    User,
    Assistant
}

