using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Cromwell.Ui;

public sealed partial class CredentialHeaderViewModel : ViewModelBase
{
    public CredentialHeaderViewModel(
        CredentialNotify credential,
        IAvaloniaReadOnlyList<InannaCommand> multiCommands,
        IInannaViewModelFactory factory
    )
    {
        Credential = credential;
        AdaptiveButtons = factory.CreateAdaptiveButtons(credential.Commands);
        MultiAdaptiveButtons = factory.CreateAdaptiveButtons(multiCommands);
    }

    public CredentialNotify Credential { get; }
    public AdaptiveButtonsViewModel AdaptiveButtons { get; }
    public AdaptiveButtonsViewModel MultiAdaptiveButtons { get; }

    [ObservableProperty]
    private bool _isMulti;
}
