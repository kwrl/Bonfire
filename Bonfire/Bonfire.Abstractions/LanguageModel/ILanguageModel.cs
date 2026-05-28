namespace Bonfire.Abstractions.LanguageModel;

/// <summary>
/// A simple interface for performing LLM chat completions.
/// </summary>
public interface ILanguageModel
{
    /// <summary>
    /// Sends a sequence of messages to the LLM and returns the completion text.
    /// </summary>
    Task<string> CompleteAsync(IEnumerable<ChatMessage> messages);
}

