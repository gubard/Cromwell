using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public sealed partial class ChangeParentCredentialViewModel : ViewModelBase
{
    public ChangeParentCredentialViewModel(
        ICromwellViewModelFactory factory,
        ISafeExecuteWrapper safeExecuteWrapper
    )
        : base(safeExecuteWrapper)
    {
        Tree = factory.CreateCredentialTree();
    }

    public CredentialTreeViewModel Tree { get; }

    [ObservableProperty]
    private bool _isRoot;
}
