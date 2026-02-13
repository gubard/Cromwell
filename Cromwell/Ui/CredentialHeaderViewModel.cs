using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Inanna.Models;

namespace Cromwell.Ui;

public sealed partial class CredentialHeaderViewModel : ViewModelBase
{
    public CredentialHeaderViewModel(
        CredentialNotify credential,
        IEnumerable<InannaCommand> multiCommands,
        AvaloniaList<InannaCommand> commands
    )
    {
        Credential = credential;
        _commands = commands;
        _multiCommands = new(multiCommands);
    }

    public CredentialNotify Credential { get; }
    public IEnumerable<InannaCommand> MultiCommands => _multiCommands;
    public IEnumerable<InannaCommand> Commands => _commands;

    private readonly AvaloniaList<InannaCommand> _commands;
    private readonly AvaloniaList<InannaCommand> _multiCommands;

    [ObservableProperty]
    private bool _isMulti;
}
