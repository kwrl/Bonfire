using System.Reflection;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using Bonfire.Abstractions.Decompilation;

namespace Bonfire.Decompiler;

/// <summary>
/// ILSpy-based type decompiler that can decompile any type into C# source.
/// </summary>
public class IlSpyTypeDecompiler : ITypeDecompiler
{
    public string DecompileType(Type type)
    {
        if (string.IsNullOrWhiteSpace(type.Assembly.Location))
            throw new InvalidOperationException($"Assembly for '{type.FullName}' does not have a physical location.");

        var module = new PEFile(type.Assembly.Location);
        var targetFramework = module.DetectTargetFrameworkId();
        var resolver = new UniversalAssemblyResolver(type.Assembly.Location, false, targetFramework);

        var settings = new DecompilerSettings
        {
            UseNestedDirectoriesForNamespaces = false,
        };

        var decompiler = new CSharpDecompiler(type.Assembly.Location, resolver, settings);
        var fullName = type.IsGenericType && !type.IsGenericTypeDefinition
            ? type.GetGenericTypeDefinition().FullName!
            : type.FullName!;

        var decompiled = decompiler.DecompileTypeAsString(new FullTypeName(fullName));

        return decompiled.Trim();
    }
}

