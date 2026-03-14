using Cromwell.Controls;
using Cromwell.Models;

namespace Cromwell.Ui;

public sealed partial class RootCredentialsView : CredentialDropUserControl, ICredentialListView
{
    public RootCredentialsView()
    {
        InitializeComponent();
    }

    public ICredentialListViewModel CredentialListViewModel =>
        DataContext as ICredentialListViewModel ?? throw new InvalidOperationException();
}
