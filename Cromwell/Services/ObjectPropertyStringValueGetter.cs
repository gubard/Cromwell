namespace Cromwell.Services;

public interface IObjectPropertyStringValueGetter
{
    string? FindStringValue(object obj, ReadOnlySpan<char> propertyName);
}

public class ObjectPropertyStringValueGetter : IObjectPropertyStringValueGetter
{
    public string? FindStringValue(object obj, ReadOnlySpan<char> propertyName)
    {
        return null;
    }
}