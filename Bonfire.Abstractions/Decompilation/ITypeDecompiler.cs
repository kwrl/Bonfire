namespace Bonfire.Abstractions.Decompilation;

/// <summary>
/// Decompiles any type into its C# source code representation.
/// </summary>
public interface ITypeDecompiler
{
    /// <summary>
    /// Decompiles the given type into C# source code.
    /// </summary>
    /// <param name="type">The type to decompile.</param>
    /// <returns>The C# source code representation of the type.</returns>
    string DecompileType(Type type);
}

