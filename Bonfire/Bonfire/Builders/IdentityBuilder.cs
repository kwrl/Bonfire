using Bonfire.Abstractions;

namespace Bonfire.Builders;

public class IdentityBuilder<T>(T instance) : IBuilder<T>
{
    public T Build()
    {
        return instance;
    }
}