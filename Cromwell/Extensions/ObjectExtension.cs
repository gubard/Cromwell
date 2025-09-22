using System.Runtime.CompilerServices;

namespace Cromwell.Extensions;

public static class ObjectExtension
{
    public static T ThrowIfNull<T>(this T? obj, [CallerArgumentExpression(nameof(obj))] string paramName = "")
    {
        if (obj is null)
        {
            throw new ArgumentNullException(paramName);
        }

        return obj;
    }
}