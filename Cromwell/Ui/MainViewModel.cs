using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;
using Inanna.Ui;

namespace Cromwell.Ui;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty] private bool _isShowPane;

    public MainViewModel(
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        RootCredentialsViewModel rootCredentials,
        PaneViewModel pane
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        Pane = pane;
        Stack.PushView(rootCredentials);
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
    public PaneViewModel Pane { get; }
}