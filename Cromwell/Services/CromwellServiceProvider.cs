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
using Microsoft.EntityFrameworkCore;
using Nestor.Db.Sqlite;
using Nestor.Db.Sqlite.Helpers;
using Turtle.Contract.Models;
using Turtle.Contract.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProviderModule]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(RootCredentialsViewModel))]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IAppSettingService), typeof(AppSettingService))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(IUiCredentialService), Factory = nameof(GetUiCredentialService))]
[Transient(typeof(INotificationService), Factory = nameof(GetNotificationService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Singleton(typeof(IAppResourceService), typeof(AppResourceService))]
[Singleton(typeof(IStringFormater), typeof(StringFormater))]
[Singleton(typeof(IObjectPropertyStringValueGetter), typeof(ObjectPropertyStringValueGetter))]
[Singleton(typeof(IDragAndDropService), typeof(DragAndDropService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(DbContext), Factory = nameof(GetDbContext))]
[Singleton(typeof(IServiceProvider), Factory = nameof(GetServiceProvider))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(IStorageService), typeof(StorageService))]
[Singleton(typeof(StackViewModel))]
public interface ICromwellServiceProvider
{
    public static IUiCredentialService GetUiCredentialService(CredentialServiceOptions options, ITryPolicyService tryPolicyService, IFactory<Memory<HttpHeader>> headersFactory, AppState appState)
    {
        return new UiCredentialService(new CredentialHttpService(new()
        {
            BaseAddress = new(options.Url),
        }, new()
        {
            TypeInfoResolver = TurtleJsonContext.Resolver,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        }, tryPolicyService, headersFactory), new EfCredentialService(new FileInfo($"./storage/Cromwell/{appState.User.ThrowIfNull().Id}.db").InitDbContext()), appState);
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

    public static DbContext GetDbContext(IStorageService storageService)
    {
        var appDirectory = storageService.GetAppDirectory();

        if (!appDirectory.Exists)
        {
            appDirectory.Create();
        }

        var file = new FileInfo(Path.Combine(appDirectory.FullName, "Cromwell.db"));

        var options = new DbContextOptionsBuilder<SqliteNestorDbContext>()
           .UseSqlite($"Data Source={file}")
           .Options;

        return new SqliteNestorDbContext(options);
    }
}