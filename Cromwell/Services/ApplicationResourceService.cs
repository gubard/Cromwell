using Avalonia;

namespace Cromwell.Services;

public interface IApplicationResourceService
{
    T GetResource<T>(string key);
}

public class ApplicationResourceService : IApplicationResourceService
{
    private readonly Application _application;

    public ApplicationResourceService(Application application)
    {
        _application = application;
    }

    public T GetResource<T>(string key)
    {
        _application.TryGetResource(key, null, out var value);

        if (value is null)
        {
            throw new NullReferenceException($"Resource {key} not found");
        }

        return (T)value;
    }
}