using Cromwell.Db;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;
using Nestor.Db.Models;

namespace Cromwell.Services;

public interface IAppSettingService
{
    Task<AppSettingEntity> GetAppSettingsAsync();
    ValueTask SaveAppSettingsAsync(AppSettingEntity setting, CancellationToken cancellationToken);
}

public class AppSettingService : IAppSettingService
{
    private readonly DbContext _dbContext;

    public AppSettingService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<AppSettingEntity> GetAppSettingsAsync()
    {
        var settings = AppSettingEntity.FindAppSettingEntity(Guid.Empty, _dbContext.Set<EventEntity>());

        return Task.FromResult(settings
         ?? new AppSettingEntity
            {
                GeneralKey = Guid.CreateVersion7().ToString().ToUpper(),
                Id = Guid.Empty,
            });
    }

    public async ValueTask SaveAppSettingsAsync(AppSettingEntity setting, CancellationToken cancellationToken)
    {
        await AppSettingEntity.EditAppSettingEntitysAsync(_dbContext, "App", [
            new(Guid.Empty)
            {
                IsEditGeneralKey = true,
                GeneralKey = setting.GeneralKey,
                IsEditTheme = true,
                Theme = setting.Theme,
            },
        ], cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}