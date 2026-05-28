using Bonfire.Builders;
using Bonfire.Compiler;
using Bonfire.Decompiler;
using Bonfire.LanguageModel.OpenAi;
using OpenAI.Chat;
using System.ClientModel;

namespace Bonfire.Demo;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("MISTRAL_API_KEY")
                     ?? throw new InvalidOperationException("Set the MISTRAL_API_KEY environment variable.");

        var chatClient = new ChatClient(
            "devstral-2512",
            new ApiKeyCredential(apiKey),
            new OpenAI.OpenAIClientOptions { Endpoint = new Uri("https://api.mistral.ai/v1") });

        var bonfire = new BonfireBuilder()
            .WithTypeCompiler(new RoslynTypeCompiler())
            .WithTypeDecompiler(new IlSpyTypeDecompiler())
            .WithLanguageModel(new OpenAiLanguageModel(chatClient))
            .WithKnownTypes(new KnownTypesBuilder()
                .AddWithSurface<ILeftPadder>())
            .Build();

        Console.WriteLine("Generating type ...");
        var padderType = await bonfire.GenerateSubTypeAsync(typeof(ILeftPadder));
        var padder = (ILeftPadder)Activator.CreateInstance(padderType)!;
    
        Console.WriteLine(padder.Pad("Bonfire", 10, '*'));
    }
}
