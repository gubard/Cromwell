using Cromwell.Db;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;

namespace Cromwell.Services;

public interface ICredentialService
{
    ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken);
    ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken);
    ValueTask<CredentialEntity[]> GetAsync(CancellationToken cancellationToken);
    ValueTask EditAsync(EditCredentialEntity[] edits, CancellationToken cancellationToken);
    ValueTask<CredentialEntity[]> GetRootsAsync(CancellationToken cancellationToken);
    ValueTask<CredentialEntity[]> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken);

    ValueTask ChangeOrderAsync(
        Guid itemId,
        Guid[] entityIds,
        bool isAfter,
        CancellationToken cancellationToken
    );
}

public class CredentialService : ICredentialService
{
    private readonly DbContext _dbContext;

    public CredentialService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        await CredentialEntity.DeleteCredentialEntitysAsync(_dbContext, "App", cancellationToken, id);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken)
    {
        await CredentialEntity.AddCredentialEntitysAsync(_dbContext, "App", cancellationToken, entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public ValueTask<CredentialEntity[]> GetAsync(CancellationToken cancellationToken)
    {
        return CredentialEntity.GetCredentialEntitysAsync(_dbContext.Set<EventEntity>(), cancellationToken);
    }

    public async ValueTask EditAsync(EditCredentialEntity[] edits, CancellationToken cancellationToken)
    {
        var credentials =
            await CredentialEntity.GetCredentialEntitysAsync(_dbContext.Set<EventEntity>(), cancellationToken);

        edits.Where(x => x is { IsEditParentId: true, ParentId: not null })
           .ToList()
           .ForEach(x => CheckParentId(credentials, credentials.Single(y => y.Id == x.Id), x.ParentId.Value));
        
        await CredentialEntity.EditCredentialEntitysAsync(_dbContext, "App", edits, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public ValueTask<CredentialEntity[]> GetRootsAsync(CancellationToken cancellationToken)
    {
        var events = _dbContext.Set<EventEntity>();

        var ids = events.Where(y => events.GroupBy(x => x.EntityId)
               .Select(y =>
                    y.Where(x =>
                            x.EntityId == y.Key
                         && x.EntityProperty == nameof(CredentialEntity.ParentId)
                         && x.EntityType == nameof(CredentialEntity))
                       .Max(x => x.Id))
               .Contains(y.Id))
           .Where(x => x.EntityGuidValue == null)
           .Select(x => x.EntityId);

        return CredentialEntity.GetCredentialEntitysAsync(events.Where(x => ids.Contains(x.EntityId)),
            cancellationToken);
    }

    public ValueTask<CredentialEntity[]> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken)
    {
        var events = _dbContext.Set<EventEntity>();

        var ids = events.Where(y => events.GroupBy(x => x.EntityId)
               .Select(y =>
                    y.Where(x =>
                            x.EntityId == y.Key
                         && x.EntityProperty == nameof(CredentialEntity.ParentId)
                         && x.EntityType == nameof(CredentialEntity))
                       .Max(x => x.Id))
               .Contains(y.Id))
           .Where(x => x.EntityGuidValue == parentId)
           .Select(x => x.EntityId);

        return CredentialEntity.GetCredentialEntitysAsync(events.Where(x => ids.Contains(x.EntityId)),
            cancellationToken);
    }

    public async ValueTask ChangeOrderAsync(
        Guid itemId,
        Guid[] entityIds,
        bool isAfter,
        CancellationToken cancellationToken
    )
    {
        var credentials =
            await CredentialEntity.GetCredentialEntitysAsync(_dbContext.Set<EventEntity>(), cancellationToken);

        var item = credentials.Single(x => x.Id == itemId);
        var items = credentials.Where(x => x.ParentId == item.ParentId && !entityIds.Contains(x.Id));

        items = isAfter
            ? items.Where(x => x.OrderIndex > item.OrderIndex)
            : items.Where(x => x.OrderIndex >= item.OrderIndex);

        items = entityIds.Select(x => credentials.Single(y => y.Id == x)).Concat(items).ToArray();
        var startIndex = isAfter ? item.OrderIndex + 1 : item.OrderIndex;

        foreach (var i in items)
        {
            i.OrderIndex = startIndex;
            startIndex++;
        }

        if (item.ParentId.HasValue)
        {
            items.Where(x => x.ParentId != item.ParentId)
               .ToList()
               .ForEach(x => CheckParentId(credentials, x, item.ParentId.Value));
        }

        await CredentialEntity.EditCredentialEntitysAsync(_dbContext, "App", items.Select(x=> new EditCredentialEntity(x.Id)
        {
            OrderIndex = x.OrderIndex,
            IsEditOrderIndex = true,
            IsEditParentId = x.ParentId != item.ParentId,
            ParentId = item.ParentId,
        }), cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private void CheckParentId(CredentialEntity[] credentials, CredentialEntity item, Guid newParentId)
    {
        if (item.Id == newParentId)
        {
            throw new($"Parent Id {newParentId} already exists");
        }
        
        var newRoot = credentials.Single(x => x.Id == newParentId);

        if (newRoot.Id == item.Id)
        {
            throw new($"Parent Id {newParentId} already exists");
        }

        CheckParentId(credentials, item, newRoot);
    }
    
    private void CheckParentId(CredentialEntity[] credentials, CredentialEntity item, CredentialEntity parent)
    {
        if (parent.ParentId is null)
        {
            return;
        }
        
        var newRoot = credentials.Single(x => x.Id == parent.ParentId);

        if (newRoot.Id == item.Id)
        {
            throw new($"Parent Id {parent.Id} already exists");
        }
        
        CheckParentId(credentials, item, newRoot);
    }
}