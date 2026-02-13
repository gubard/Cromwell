using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Inanna.Models;

namespace Cromwell.Ui;

public sealed partial class CredentialHeaderViewModel : ViewModelBase
{
    public CredentialHeaderViewModel(
        CredentialNotify credential,
        IEnumerable<InannaCommand> multiCommands,
        IEnumerable<InannaCommand> commands
    )
    {
        Credential = credential;
        Commands = commands;
        MultiCommands = multiCommands;
    }

    public CredentialNotify Credential { get; }
    public IEnumerable<InannaCommand> MultiCommands { get; }
    public IEnumerable<InannaCommand> Commands { get; }

    [ObservableProperty]
    private bool _isMulti;
}
