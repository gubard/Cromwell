namespace Cromwell.Models;

public readonly struct GeneratePasswordOptions
{
    public GeneratePasswordOptions(string availableCharacters, ushort length, string regex)
    {
        AvailableCharacters = availableCharacters;
        Length = length;
        Regex = regex;
    }

    public string AvailableCharacters { get; }
    public ushort Length { get; }
    public string Regex { get; }
}
