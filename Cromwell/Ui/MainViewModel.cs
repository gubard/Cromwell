using Inanna.Models;
using Inanna.Ui;

namespace Cromwell.Ui;

public partial class MainViewModel : ViewModelBase
{
    public MainViewModel(
        StackViewModel stack,
        NavigationBarViewModel navigationBar,
        CredentialsTreeViewModel credentialsTree
    )
    {
        Stack = stack;
        NavigationBar = navigationBar;
        Stack.PushView(credentialsTree);
    }

    public StackViewModel Stack { get; }
    public NavigationBarViewModel NavigationBar { get; }
}