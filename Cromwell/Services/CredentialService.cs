using Cromwell.Db;
using Microsoft.EntityFrameworkCore;

namespace Cromwell.Services;

public class CredentialService : ICredentialService
{
    private readonly DbContext _dbContext;

    public CredentialService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken)
    {
        return CredentialEntity.AddCredentialEntitysAsync(_dbContext, "App", cancellationToken, entity);
    }
}