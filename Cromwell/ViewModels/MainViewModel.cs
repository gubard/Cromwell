using CommunityToolkit.Mvvm.ComponentModel;

namespace Cromwell.ViewModels;

public class MainViewModel : ObservableObject
{
    public MainViewModel(EditCredentialViewModel editCredentialViewModel)
    {
        EditCredentialViewModel = editCredentialViewModel;
    }

    public EditCredentialViewModel EditCredentialViewModel { get; }
}