using Bonfire.Abstractions;
using Bonfire.Abstractions.Compilation;
using Bonfire.Abstractions.Decompilation;
using Bonfire.Abstractions.LanguageModel;

namespace Bonfire.Builders;

public class BonfireBuilder : IBuilder<Bonfire>
{
    private IBuilder<ITypeCompiler>? TypeCompilerBuilder { get; init; }
    private IBuilder<ITypeDecompiler>? TypeDecompilerBuilder { get; init; }
    private IBuilder<ILanguageModel>? LanguageModelBuilder { get; init; }
    private KnownTypesBuilder? KnownTypesBuilder { get; init; }

    public BonfireBuilder() { }

    private BonfireBuilder(
        IBuilder<ITypeCompiler>? typeCompilerBuilder,
        IBuilder<ITypeDecompiler>? typeDecompilerBuilder,
        IBuilder<ILanguageModel>? languageModelBuilder,
        KnownTypesBuilder? knownTypesBuilder)
    {
        TypeCompilerBuilder = typeCompilerBuilder;
        TypeDecompilerBuilder = typeDecompilerBuilder;
        LanguageModelBuilder = languageModelBuilder;
        KnownTypesBuilder = knownTypesBuilder;
    }

    public BonfireBuilder WithTypeCompiler(ITypeCompiler typeCompiler)
    {
        return new BonfireBuilder(new IdentityBuilder<ITypeCompiler>(typeCompiler), TypeDecompilerBuilder, LanguageModelBuilder, KnownTypesBuilder);
    }
    
    public BonfireBuilder WithTypeCompiler(IBuilder<ITypeCompiler> typeCompilerBuilder)
    {
        return new BonfireBuilder(typeCompilerBuilder, TypeDecompilerBuilder, LanguageModelBuilder, KnownTypesBuilder);
    }

    public BonfireBuilder WithTypeDecompiler(ITypeDecompiler typeDecompiler)
    {
        return new BonfireBuilder(TypeCompilerBuilder, new IdentityBuilder<ITypeDecompiler>(typeDecompiler), LanguageModelBuilder, KnownTypesBuilder);
    }

    public BonfireBuilder WithTypeDecompiler(IBuilder<ITypeDecompiler> typeDecompiler)
    {
        return new BonfireBuilder(TypeCompilerBuilder, typeDecompiler, LanguageModelBuilder, KnownTypesBuilder);
    }

    public BonfireBuilder WithLanguageModel(ILanguageModel languageModel)
    {
        return new BonfireBuilder(TypeCompilerBuilder, TypeDecompilerBuilder, new IdentityBuilder<ILanguageModel>(languageModel), KnownTypesBuilder);
    }

    public BonfireBuilder WithLanguageModel(IBuilder<ILanguageModel> languageModel)
    {
        return new BonfireBuilder(TypeCompilerBuilder, TypeDecompilerBuilder, languageModel, KnownTypesBuilder);
    }

    public BonfireBuilder WithKnownTypes(KnownTypesBuilder knownTypesBuilder)
    {
        return new BonfireBuilder(TypeCompilerBuilder, TypeDecompilerBuilder, LanguageModelBuilder, knownTypesBuilder);
    }
    
    public Bonfire Build()
    {
        var typeCompiler = TypeCompilerBuilder?.Build() ?? throw new InvalidOperationException("TypeCompiler is required.");
        var typeDecompiler = TypeDecompilerBuilder?.Build() ?? throw new InvalidOperationException("TypeDecompiler is required.");
        var languageModel = LanguageModelBuilder?.Build() ?? throw new InvalidOperationException("LanguageModel is required.");
        var knownTypes = KnownTypesBuilder?.Build() ?? new KnownTypes();
        
        return new Bonfire(typeCompiler, typeDecompiler, languageModel, knownTypes);
    }
}
