using Avalonia;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Services;
using Jab;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProviderModule]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(INotificationService), Factory = nameof(GetNotificationService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Transient(typeof(ICromwellViewModelFactory), typeof(CromwellViewModelFactory))]
[Singleton(typeof(IAppResourceService), typeof(AppResourceService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(IServiceProvider), Factory = nameof(GetServiceProvider))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(CromwellCommands))]
[Singleton(typeof(ICredentialMemoryCache), typeof(CredentialMemoryCache))]
public interface ICromwellServiceProvider
{
    public static INotificationService GetNotificationService(ICommandFactory commandFactory)
    {
        return new NotificationService("Notifications", TimeSpan.FromSeconds(5), commandFactory);
    }

    public static IDialogService GetDialogService(
        IAppResourceService appResourceService,
        ICommandFactory commandFactory,
        IInannaViewModelFactory factory
    )
    {
        return new DialogService("MessageBox", appResourceService, commandFactory, factory);
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
