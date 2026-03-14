using Cromwell.Services;

namespace Cromwell.Models;

public interface ICredentialListView
{
    ICredentialListViewModel CredentialListViewModel { get; }
}

public interface ICredentialListViewModel
{
    CromwellCommands CromwellCommands { get; }
}
