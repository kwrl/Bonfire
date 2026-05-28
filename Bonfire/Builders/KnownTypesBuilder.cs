using System.Reflection;
using Bonfire.Abstractions;

namespace Bonfire;

/// <summary>
/// Builder for constructing a KnownTypes collection.
/// </summary>
public class KnownTypesBuilder : IBuilder<KnownTypes>
{
    private readonly KnownTypes _types = new();

    /// <summary>
    /// Adds a single type to the known types.
    /// </summary>
    public KnownTypesBuilder Add<T>()
    {
        _types.Add(typeof(T));
        return this;
    }

    /// <summary>
    /// Adds a single type to the known types.
    /// </summary>
    public KnownTypesBuilder Add(Type type)
    {
        _types.Add(type);
        return this;
    }

    /// <summary>
    /// Adds a type and all types exposed through its public surface area:
    /// interfaces, base types, generic arguments, method return/parameter types, and property types.
    /// </summary>
    public KnownTypesBuilder AddWithSurface<T>()
    {
        return AddWithSurface(typeof(T));
    }

    /// <summary>
    /// Adds a type and all types exposed through its public surface area:
    /// interfaces, base types, generic arguments, method return/parameter types, and property types.
    /// </summary>
    public KnownTypesBuilder AddWithSurface(Type type)
    {
        CollectSurfaceTypes(type, _types);
        return this;
    }

    public KnownTypes Build() => _types;

    private static void CollectSurfaceTypes(Type type, HashSet<Type> discovered)
    {
        if (type == null || IsFrameworkType(type) || !discovered.Add(type))
            return;

        if (type.IsGenericType)
        {
            foreach (var arg in type.GetGenericArguments())
                CollectSurfaceTypes(arg, discovered);
        }

        foreach (var iface in type.GetInterfaces())
            CollectSurfaceTypes(iface, discovered);

        if (type.BaseType != null)
            CollectSurfaceTypes(type.BaseType, discovered);

        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
        {
            CollectSurfaceTypes(method.ReturnType, discovered);
            foreach (var param in method.GetParameters())
                CollectSurfaceTypes(param.ParameterType, discovered);
        }

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            CollectSurfaceTypes(prop.PropertyType, discovered);
    }

    private static bool IsFrameworkType(Type type)
    {
        if (type.IsPrimitive || type == typeof(string) || type == typeof(object) ||
            type == typeof(void) || type == typeof(decimal) || type == typeof(DateTime) ||
            type == typeof(DateTimeOffset) || type == typeof(TimeSpan) || type == typeof(Guid) ||
            type == typeof(Uri))
            return true;

        var ns = type.Namespace;
        if (ns == null)
            return false;

        return ns.StartsWith("System") || ns.StartsWith("Microsoft");
    }
}

