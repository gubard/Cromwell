using Inanna.Models;

namespace Cromwell.Ui;

public class CredentialHeaderViewModel : ViewModelBase
{
    public CredentialHeaderViewModel(CredentialParametersViewModel credential)
    {
        Credential = credential;
    }

    public CredentialParametersViewModel Credential { get; }
}