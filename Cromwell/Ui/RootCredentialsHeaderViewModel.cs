using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Cromwell.Ui;

public sealed partial class RootCredentialsHeaderViewModel : ViewModelBase
{
    public RootCredentialsHeaderViewModel(
        IAvaloniaReadOnlyList<InannaCommand> commands,
        IInannaViewModelFactory factory
    )
    {
        AdaptiveButtons = factory.CreateAdaptiveButtons(commands);
    }

    public AdaptiveButtonsViewModel AdaptiveButtons { get; }

    [ObservableProperty]
    private bool _isMulti;
}
