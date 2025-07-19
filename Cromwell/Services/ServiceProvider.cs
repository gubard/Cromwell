using Avalonia;
using Cromwell.Ui;
using Jab;

namespace Cromwell.Services;

public interface IServiceProvider
{
    T GetService<T>();
}

[ServiceProvider]
[Transient(typeof(MainViewModel))]
[Singleton(typeof(IDialogService), typeof(DialogService))]
[Singleton(typeof(IApplicationResourceService), typeof(ApplicationResourceService))]
[Singleton(typeof(IStringFormater), typeof(StringFormater))]
[Singleton(typeof(IObjectPropertyStringValueGetter), typeof(ObjectPropertyStringValueGetter))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
public partial class ServiceProvider : IServiceProvider
{
    public static Application GetApplication()
    {
        return Application.Current ?? throw new NullReferenceException("Application not found");
    }
}