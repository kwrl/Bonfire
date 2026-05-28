using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Bonfire.Abstractions.Compilation;

namespace Bonfire.Compiler;

/// <summary>
/// Compiles C# source code at runtime using Roslyn.
/// </summary>
public class RoslynTypeCompiler : ITypeCompiler
{
    private static int _assemblyCounter = 0;

    public async Task<Type> CompileAsync(string sourceCode)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var assemblyName = $"GeneratedAssembly_{_assemblyCounter++}_{Guid.NewGuid():N}";

        var references = GetReferences();

        var compilation = CSharpCompilation.Create(assemblyName)
            .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
            .AddReferences(references)
            .AddSyntaxTrees(syntaxTree);

        using var ms = new MemoryStream();
        var emitResult = await Task.Run(() => compilation.Emit(ms));

        if (!emitResult.Success)
        {
            var diagnostics = string.Join(Environment.NewLine,
                emitResult.Diagnostics.Select(d => $"{d.Severity}: {d.GetMessage()}"));
            throw new InvalidOperationException(
                $"Compilation failed:{Environment.NewLine}{diagnostics}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        var assembly = Assembly.Load(ms.ToArray());

        var types = assembly.GetExportedTypes();
        if (types.Length == 0)
            throw new InvalidOperationException("No public types found in compiled assembly.");
        if (types.Length > 1)
            throw new InvalidOperationException($"Expected a single type in compiled assembly, but found {types.Length}.");

        return types[0];
    }

    private static MetadataReference[] GetReferences()
    {
        var references = new List<MetadataReference>();

        var assemblyLocations = new[]
        {
            typeof(object).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Task).Assembly,
        };

        foreach (var assembly in assemblyLocations)
        {
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        var runtimeLocation = typeof(object).Assembly.Location;
        var runtimePath = Path.GetDirectoryName(runtimeLocation)!;

        var netCoreAppAssemblies = Directory.GetFiles(runtimePath, "System*.dll")
            .Take(20);

        foreach (var dll in netCoreAppAssemblies)
        {
            try
            {
                references.Add(MetadataReference.CreateFromFile(dll));
            }
            catch
            {
            }
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!string.IsNullOrEmpty(assembly.Location))
            {
                try
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
                catch
                {
                }
            }
        }

        return references.Distinct().ToArray();
    }
}

