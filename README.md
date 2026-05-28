# 🔥 Bonfire

**Runtime subtype generation for .NET — because why write implementations yourself when you can mass-generate them at industrial scale?**

Bonfire takes your interfaces, feeds them to a large language model, compiles the result, and hands you back a ready-to-use `Type` — all at runtime. It's the kind of engineering that keeps datacenters warm and SREs on their toes.

> *Named for its ability to light up both the Amazon rainforest and your production environment.*

## Getting Started

Set up a bonfire with the fluent builder:

```csharp
var bonfire = new BonfireBuilder()
    .WithTypeCompiler(new RoslynTypeCompiler())
    .WithTypeDecompiler(new IlSpyTypeDecompiler())
    .WithLanguageModel(new OpenAiLanguageModel(chatClient))
    .WithKnownTypes(new KnownTypesBuilder()
        .AddWithSurface<IMyService>())
    .Build();
```

## Generating Types

Once you have a bonfire going, toss your interfaces into it:

```csharp
var type = await bonfire.GenerateSubTypeAsync(typeof(ILeftPadder));
var padder = (ILeftPadder)Activator.CreateInstance(type)!;

Console.WriteLine(padder.Pad("hello", 10, '.'));
// .....hello
```

Need a sorting strategy? Generate one:

```csharp
var sortType = await bonfire.GenerateSubTypeAsync(typeof(ISorter));
var sorter = (ISorter)Activator.CreateInstance(sortType)!;
var sorted = sorter.Sort(new[] { 5, 3, 1, 4, 2 });
```

User repository? Sure, let the bonfire figure it out:

```csharp
var repoType = await bonfire.GenerateSubTypeAsync(typeof(IUserRepository));
var repo = (IUserRepository)Activator.CreateInstance(repoType)!;
repo.Save(new User("Ada"));
```

Validators, formatters, converters — throw them all on the pile:

```csharp
var validatorType = await bonfire.GenerateSubTypeAsync(typeof(IEmailValidator));
var validator = (IEmailValidator)Activator.CreateInstance(validatorType)!;

var formatterType = await bonfire.GenerateSubTypeAsync(typeof(IDateFormatter));
var formatter = (IDateFormatter)Activator.CreateInstance(formatterType)!;
```

## Architecture

Bonfire is composed of pluggable components:

| Component | What it does |
|---|---|
| `ITypeDecompiler` | Decompiles your interface into C# source |
| `ILanguageModel` | Asks an LLM to write an implementation |
| `ITypeCompiler` | Compiles the generated source at runtime |
| `KnownTypes` | Gives the LLM context about your type ecosystem |

Implementations ship for Roslyn (compiler), ILSpy (decompiler), and OpenAI-compatible APIs (language model).

## Contributing

Bring marshmallows. 🏕️
