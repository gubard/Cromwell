using Cromwell.Db;

namespace Cromwell.Services;

public interface ICredentialService
{
    ValueTask AddAsync(CredentialEntity entity, CancellationToken cancellationToken);
}