using Cromwell.Services;
using IServiceProvider = Cromwell.Services.IServiceProvider;

namespace Cromwell.Helpers;

public static class DiHelper
{
    static DiHelper()
    {
        ServiceProvider = new ServiceProvider();
    }

    public static IServiceProvider ServiceProvider;
}