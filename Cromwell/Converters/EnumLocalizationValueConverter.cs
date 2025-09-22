using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Cromwell.Extensions;

namespace Cromwell.Converters;

public class EnumLocalizationValueConverter : IValueConverter
{
    public static readonly EnumLocalizationValueConverter Instance = new();

    private readonly Application _application = Application.Current.ThrowIfNull();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not Enum e)
        {
            return null;
        }

        var typeName = e.GetType().Name;
        var resource = _application.GetResourceOrNull($"Lang.{typeName}.{e}");

        return resource ?? value;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}