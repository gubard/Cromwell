using Inanna.Models;
using Inanna.Ui;

namespace Cromwell.Ui;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        RootCredentialsViewModel rootCredentials
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        Stack.PushView(rootCredentials);
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
}