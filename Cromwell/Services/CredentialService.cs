using Cromwell.Db;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;

namespace Cromwell.Services;

public interface ICredentialService
{
    ValueTask DeleteAsync(Guid id, CancellationToken cancellationToken);
    ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken);
    ValueTask<CredentialEntity[]> GetAsync(CancellationToken cancellationToken);
    ValueTask ChangeParentAsync(Guid id, Guid? parent, CancellationToken cancellationToken);
    ValueTask EditAsync(EditCredentialEntity[] edits, CancellationToken cancellationToken);
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

    public async ValueTask ChangeParentAsync(Guid id, Guid? parent, CancellationToken cancellationToken)
    {
        if (id == parent)
        {
            throw new ArgumentException("Parent can't be the same as the current");
        }

        await CredentialEntity.EditCredentialEntitysAsync(_dbContext, "App", [
            new(id)
            {
                IsEditParentId = true,
                ParentId = parent,
            },
        ], cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask EditAsync(EditCredentialEntity[] edits, CancellationToken cancellationToken)
    {
        await CredentialEntity.EditCredentialEntitysAsync(_dbContext, "App", edits, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}