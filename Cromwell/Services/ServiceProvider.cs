using Avalonia;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Services;
using Inanna.Ui;
using Jab;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;
using Nestor.Db.Sqlite;
using IServiceProvider = Inanna.Services.IServiceProvider;

namespace Cromwell.Services;

[ServiceProvider]
[Transient(typeof(NavigationBarViewModel))]
[Transient(typeof(AppSettingViewModel))]
[Transient(typeof(CredentialsTreeViewModel))]
[Transient(typeof(ITransformer<string, byte[]>), typeof(StringToUtf8))]
[Transient(typeof(IAppSettingService), typeof(AppSettingService))]
[Transient(typeof(IPasswordGeneratorService), typeof(PasswordGeneratorService))]
[Transient(typeof(IClipboardService), typeof(AvaloniaClipboardService))]
[Transient(typeof(ICredentialService), typeof(CredentialService))]
[Singleton(typeof(IDialogService), Factory = nameof(GetDialogService))]
[Singleton(typeof(IApplicationResourceService), typeof(ApplicationResourceService))]
[Singleton(typeof(IStringFormater), typeof(StringFormater))]
[Singleton(typeof(IObjectPropertyStringValueGetter), typeof(ObjectPropertyStringValueGetter))]
[Singleton(typeof(IDragAndDropService), typeof(DragAndDropService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(DbContext), Factory = nameof(GetDbContext))]
[Singleton(typeof(IServiceProvider), Factory = nameof(GetServiceProvider))]
[Singleton(typeof(INavigator), typeof(Navigator))]
[Singleton(typeof(StackViewModel))]
[Singleton(typeof(MainViewModel))]
public partial class ServiceProvider : IServiceProvider
{
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

    public static DbContext GetDbContext()
    {
        var file = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "Cromwell.db"));

        return new NestorDbContext<EventEntityTypeConfiguration>(
            new DbContextOptionsBuilder<NestorDbContext<EventEntityTypeConfiguration>>()
               .UseSqlite($"Data Source={file}")
               .Options);
    }
}