using Bonfire.Abstractions.Compilation;
using Bonfire.Abstractions.Decompilation;
using Bonfire.Abstractions.LanguageModel;

namespace Bonfire;

public class Bonfire(
    ITypeCompiler compiler,
    ITypeDecompiler decompiler,
    ILanguageModel languageModel,
    KnownTypes knownTypes
)
{
    public async Task<Type> GenerateSubTypeAsync(Type superType)
    {
        var superTypeSource = decompiler.DecompileType(superType);
        var serializedKnownTypes = BuildKnownTypesContext(superType);
        var prompt = BuildPrompt(superType, superTypeSource, serializedKnownTypes);

        var messages = new[]
        {
            new ChatMessage(ChatMessageRole.System,
                "You are a C# code generator. You output ONLY valid C# source code with no markdown fences, no explanations, and no comments beyond XML docs. The code must compile without errors."),
            new ChatMessage(ChatMessageRole.User, prompt)
        };

        var response = await languageModel.CompleteAsync(messages);
        response = StripMarkdownFences(response).Trim();

        return await compiler.CompileAsync(response);
    }

    private string BuildKnownTypesContext(Type excludeType)
    {
        var sources = new List<string>();
        foreach (var type in knownTypes)
        {
            if (type == excludeType) continue;
            try
            {
                var source = decompiler.DecompileType(type);
                sources.Add(source);
            }
            catch
            {
                // Skip types that can't be decompiled
            }
        }

        return sources.Count > 0
            ? string.Join("\n\n", sources)
            : string.Empty;
    }

    private static string BuildPrompt(Type interfaceType, string interfaceSource, string knownTypes)
    {
        var className = $"{interfaceType.Name.TrimStart('I')}Impl";
        var namespaceName = interfaceType.Namespace ?? "Generated";

        var knownTypesSection = string.IsNullOrEmpty(knownTypes)
            ? string.Empty
            : $"""

            The following types are available in the runtime and may be used in your implementation:
            ```csharp
            {knownTypes}
            ```
            """;

        return $"""
            Generate a C# class that implements the following interface.
            
            Requirements:
            - The class must be named `{className}`
            - The class must be in namespace `{namespaceName}`
            - The class must be public
            - All methods should have reasonable default implementations
            - Include all necessary using statements
            - Do NOT include the interface definition, only the implementing class
            - The output must be a single file that compiles on its own (given the interface assembly is referenced)
            
            Interface source:
            ```csharp
            {interfaceSource}
            ```
            {knownTypesSection}
            """;
    }

    private static string StripMarkdownFences(string code)
    {
        if (code.StartsWith("```"))
        {
            var firstNewline = code.IndexOf('\n');
            if (firstNewline >= 0)
                code = code[(firstNewline + 1)..];
        }

        if (code.EndsWith("```"))
        {
            code = code[..^3];
        }

        return code;
    }
}
