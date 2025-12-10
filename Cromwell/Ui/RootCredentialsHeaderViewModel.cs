using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Cromwell.Ui;

public partial class RootCredentialsHeaderViewModel : ViewModelBase
{
    private readonly AvaloniaList<InannaCommand> _commands;
    [ObservableProperty] private bool _isMulti;

    public RootCredentialsHeaderViewModel(IEnumerable<InannaCommand> commands)
    {
        _commands = new(commands);
    }

    public IEnumerable<InannaCommand> Commands => _commands;
}