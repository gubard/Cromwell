using Avalonia;
using Avalonia.Controls;

namespace Cromwell.Extensions;

public static class ApplicationExtension
{
    public static object? GetResourceOrNull(this Application app, string key)
    {
        app.TryGetResource(key, out var value);

        return value;
    }
}