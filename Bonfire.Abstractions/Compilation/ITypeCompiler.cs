namespace Bonfire.Abstractions.Compilation;

/// <summary>
/// Compiles C# source code at runtime and returns the compiled types.
/// </summary>
public interface ITypeCompiler
{
    /// <summary>
    /// Compiles the given source code and returns the single compiled Type from the resulting assembly.
    /// </summary>
    /// <param name="sourceCode">The C# source code to compile.</param>
    /// <returns>The compiled Type instance.</returns>
    Task<Type> CompileAsync(string sourceCode);
}

