namespace Bonfire;

public static class BonfireExtensions
{
    public static Task<Type> GenerateSubTypeAsync<T>(this Bonfire bonfire)
    {
        return bonfire.GenerateSubTypeAsync(typeof(T));
    }
}
