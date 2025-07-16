using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Cromwell.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public MainViewModel(IEnumerable<EditCredentialViewModel> credentials)
    {
        Credentials = new(credentials);
        SelectedCredential = Credentials.FirstOrDefault();
    }

    public AvaloniaList<EditCredentialViewModel> Credentials { get; }

    [ObservableProperty]
    public partial EditCredentialViewModel? SelectedCredential { get; set; }
}