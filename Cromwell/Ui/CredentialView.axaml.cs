using Cromwell.Controls;

namespace Cromwell.Ui;

public sealed partial class CredentialView : CredentialDropUserControl
{
    public CredentialView()
    {
        InitializeComponent();
    }

    public CredentialViewModel ViewModel =>
        DataContext as CredentialViewModel ?? throw new InvalidOperationException();
}
