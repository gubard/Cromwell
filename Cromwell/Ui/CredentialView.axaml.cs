using Cromwell.Controls;
using Cromwell.Models;

namespace Cromwell.Ui;

public sealed partial class CredentialView : CredentialDropUserControl, ICredentialListView
{
    public CredentialView()
    {
        InitializeComponent();
    }

    public CredentialViewModel ViewModel =>
        DataContext as CredentialViewModel ?? throw new InvalidOperationException();

    public ICredentialListViewModel CredentialListViewModel =>
        DataContext as ICredentialListViewModel ?? throw new InvalidOperationException();
}
