using System.Text;

namespace Cromwell.Services;

public interface IStringFormater
{
    string Format(string format, params object[] parameters);
}

public class StringFormater : IStringFormater
{
    private readonly IObjectPropertyStringValueGetter _objectPropertyStringValueGetter;

    public StringFormater(IObjectPropertyStringValueGetter objectPropertyStringValueGetter)
    {
        _objectPropertyStringValueGetter = objectPropertyStringValueGetter;
    }

    public string Format(string format, params object[] parameters)
    {
        var span = format.AsSpan();
        var startIndex = span.IndexOf('{');

        if (startIndex == -1)
        {
            return format;
        }

        var result = new StringBuilder();

        while (startIndex != -1)
        {
            result.Append(span.Slice(0, startIndex));
            var endIndex = span.IndexOf('}');

            if (endIndex == -1)
            {
                break;
            }

            var str = span.Slice(startIndex + 1, endIndex - startIndex - 1);
            GetFormatValues(str, out var parameterIndex, out var propertyNameString);
            var value = GetStringValue(parameters[parameterIndex], propertyNameString);

            if (value == string.Empty)
            {
                result.Append(parameters[parameterIndex]);
            }
            else
            {
                result.Append(value ?? parameters[parameterIndex]);
            }

            var currentIndex = endIndex + 1;

            if (currentIndex >= span.Length)
            {
                return result.ToString();
            }

            span = span.Slice(currentIndex);
            startIndex = span.IndexOf('{');
        }

        result.Append(span);

        return result.ToString();
    }

    private string? GetStringValue(object parameter, ReadOnlySpan<char> stringPropertyNames)
    {
        var propertyNames = stringPropertyNames.Split(',');

        foreach (var propertyName in propertyNames)
        {
            var value = _objectPropertyStringValueGetter.FindStringValue(parameter, stringPropertyNames[propertyName]);

            if (value is not null)
            {
                return value;
            }
        }

        return null;
    }

    private void GetFormatValues(
        ReadOnlySpan<char> str,
        out int parameterIndex,
        out ReadOnlySpan<char> stringPropertyNames
    )
    {
        if (str.IsEmpty)
        {
            parameterIndex = 0;
            stringPropertyNames = str;

            return;
        }

        if (str[0] == '[')
        {
            var endIndex = str.IndexOf(']');
            parameterIndex = int.Parse(str.Slice(1, endIndex - 1));
            stringPropertyNames = str.Slice(endIndex + 1);

            return;
        }

        parameterIndex = 0;
        stringPropertyNames = str;
    }
}