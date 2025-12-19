using System.Text.Json;
using Avalonia;
using Cromwell.Models;
using Cromwell.Ui;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Jab;
using Nestor.Db.Sqlite.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProviderModule]
[Transient(typeof(RootCredentialsViewModel))]
[Transient(typeof(RootCredentialsHeaderViewModel))]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(IUiCredentialService), Factory = nameof(GetUiCredentialService))]
[Transient(typeof(INotificationService), Factory = nameof(GetNotificationService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Singleton(typeof(ICredentialCache), typeof(CredentialCache))]
[Singleton(typeof(IAppResourceService), typeof(AppResourceService))]
[Singleton(typeof(IDragAndDropService), typeof(DragAndDropService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(IServiceProvider), Factory = nameof(GetServiceProvider))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(StackViewModel))]
public interface ICromwellServiceProvider
{
    public static IUiCredentialService GetUiCredentialService(
        CredentialServiceOptions options,
        ITryPolicyService tryPolicyService,
        IFactory<Memory<HttpHeader>> headersFactory,
        AppState appState,
        ICredentialCache cache,
        INavigator navigator,
        IStorageService storageService,
        GaiaValues gaiaValues
    )
    {
        return new UiCredentialService(
            new HttpCredentialService(
                new() { BaseAddress = new(options.Url) },
                new()
                {
                    TypeInfoResolver = TurtleJsonContext.Resolver,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                },
                tryPolicyService,
                headersFactory
            ),
            new EfCredentialService(
                new FileInfo(
                    $"{storageService.GetAppDirectory()}/Cromwell/{appState.User.ThrowIfNull().Id}.db"
                ).InitDbContext(),
                gaiaValues
            ),
            appState,
            cache,
            navigator
        );
    }

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
