using Cromwell.Models;
using Inanna.Models;

namespace Cromwell.Ui;

public class CredentialHeaderViewModel : ViewModelBase
{
    public CredentialHeaderViewModel(CredentialNotify credential)
    {
        Credential = credential;
    }

    public CredentialNotify Credential { get; }
}