using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Inanna.Models;

namespace Cromwell.Ui;

public sealed partial class CredentialHeaderViewModel : ViewModelBase
{
    private readonly AvaloniaList<InannaCommand> _commands;
    [ObservableProperty] private bool _isMulti;

    public CredentialHeaderViewModel(CredentialNotify credential, IEnumerable<InannaCommand> commands)
    {
        Credential = credential;
        _commands = new(commands);
    }

    public CredentialNotify Credential { get; }
    public IEnumerable<InannaCommand> Commands => _commands;
}