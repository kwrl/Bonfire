namespace Bonfire.Demo;

/// <summary>
/// Pretty-prints a given text string to the console using decorative formatting
/// such as borders, colors, or ASCII art embellishments.
/// </summary>
public interface ILeftPadder
{
    string Pad(string text, int length, char paddingChar);
}

