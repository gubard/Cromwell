using Avalonia;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Services;
using Inanna.Ui;
using Jab;
using Microsoft.EntityFrameworkCore;
using Nestor.Db.Sqlite;
using IServiceProvider = Inanna.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProviderModule]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(RootCredentialsViewModel))]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IAppSettingService), typeof(AppSettingService))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(ICredentialService), typeof(CredentialService))]
[Transient(typeof(INotificationService), Factory = nameof(GetNotificationService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Singleton(typeof(IApplicationResourceService), typeof(ApplicationResourceService))]
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