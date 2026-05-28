namespace Bonfire.Abstractions;

public interface IBuilder<out T>
{
    T Build();
}