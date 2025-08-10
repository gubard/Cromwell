using Avalonia;
using Cromwell.Ui;
using Jab;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;
using Nestor.Db.Sqlite;

namespace Cromwell.Services;

public interface IServiceProvider
{
    T GetService<T>();
}

[ServiceProvider]
[Transient(typeof(MainViewModel))]
[Transient(typeof(ICredentialService), typeof(CredentialService))]
[Singleton(typeof(IDialogService), typeof(DialogService))]
[Singleton(typeof(IApplicationResourceService), typeof(ApplicationResourceService))]
[Singleton(typeof(IStringFormater), typeof(StringFormater))]
[Singleton(typeof(IObjectPropertyStringValueGetter), typeof(ObjectPropertyStringValueGetter))]
[Singleton(typeof(IDragAndDropService), typeof(DragAndDropService))]
[Singleton(typeof(Application), Factory = nameof(GetApplication))]
[Singleton(typeof(DbContext), Factory = nameof(GetDbContext))]
public partial class ServiceProvider : IServiceProvider
{
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