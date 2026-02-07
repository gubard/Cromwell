using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Services;
using Inanna.Models;

namespace Cromwell.Ui;

public sealed partial class ChangeParentCredentialViewModel : ViewModelBase
{
    public ChangeParentCredentialViewModel(ICromwellViewModelFactory factory)
    {
        Tree = factory.CreateCredentialTree();
    }

    public CredentialTreeViewModel Tree { get; }

    [ObservableProperty]
    private bool _isRoot;
}
