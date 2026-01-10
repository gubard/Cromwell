using Avalonia;
using Cromwell.Ui;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Services;
using Inanna.Ui;
using Jab;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProviderModule]
[Transient(typeof(RootCredentialsViewModel))]
[Transient(typeof(RootCredentialsHeaderViewModel))]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(INotificationService), Factory = nameof(GetNotificationService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Singleton(typeof(ICredentialMemoryCache), typeof(CredentialMemoryCache))]
[Singleton(typeof(IAppResourceService), typeof(AppResourceService))]
[Singleton(typeof(IDragAndDropService), typeof(DragAndDropService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(IServiceProvider), Factory = nameof(GetServiceProvider))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(StackViewModel))]
public interface ICromwellServiceProvider
{
    public static INotificationService GetNotificationService()
    {
        return new NotificationService("Notifications", TimeSpan.FromSeconds(5));
    }

    public static IDialogService GetDialogService()
    {
        return new DialogService("MessageBox");
    }

    public static IServiceProvider GetServiceProvider()
    {
        return DiHelper.ServiceProvider;
    }

    public static Application GetApplication()
    {
        return Application.Current ?? throw new NullReferenceException("Application not found");
    }
}
