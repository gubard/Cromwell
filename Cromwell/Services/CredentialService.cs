using Cromwell.Db;
using Microsoft.EntityFrameworkCore;
using Nestor.Db;

namespace Cromwell.Services;

public interface ICredentialService
{
    ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken);
    ValueTask<CredentialEntity[]> GetAsync(CancellationToken cancellationToken);
}

public class CredentialService : ICredentialService
{
    private readonly DbContext _dbContext;

    public CredentialService(DbContext dbContext)
    {
        _dbContext = dbContext;
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
}