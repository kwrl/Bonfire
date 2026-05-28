using Microsoft.Extensions.DependencyInjection;

namespace Bonfire.DependencyInjection;

/// <summary>
/// Extension methods for populating a <see cref="KnownTypesBuilder"/> from an <see cref="IServiceCollection"/>.
/// </summary>
public static class KnownTypesBuilderExtensions
{
    /// <summary>
    /// Adds all service types registered in the <see cref="IServiceCollection"/> as known types.
    /// This gives Bonfire context about every type available in the DI container
    /// so the LLM can reference them when generating implementations.
    /// </summary>
    /// <param name="builder">The known types builder.</param>
    /// <param name="services">The service collection to extract types from.</param>
    /// <returns>The builder for chaining.</returns>
    public static KnownTypesBuilder AddServiceTypes(
        this KnownTypesBuilder builder,
        IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            builder.Add(descriptor.ServiceType);

            if (descriptor.ImplementationType is not null)
            {
                builder.Add(descriptor.ImplementationType);
            }
        }

        return builder;
    }

    /// <summary>
    /// Adds all service types registered in the <see cref="IServiceCollection"/> as known types,
    /// including their full public surface area (interfaces, base types, method signatures, and property types).
    /// </summary>
    /// <param name="builder">The known types builder.</param>
    /// <param name="services">The service collection to extract types from.</param>
    /// <returns>The builder for chaining.</returns>
    public static KnownTypesBuilder AddServiceTypesWithSurface(
        this KnownTypesBuilder builder,
        IServiceCollection services)
    {
        foreach (var descriptor in services)
        {
            builder.AddWithSurface(descriptor.ServiceType);

            if (descriptor.ImplementationType is not null)
            {
                builder.AddWithSurface(descriptor.ImplementationType);
            }
        }

        return builder;
    }
}
