using Avalonia.Controls;

namespace Cromwell.Ui;

public partial class DialogView : UserControl
{
    public DialogView()
    {
        InitializeComponent();
    }

    public DialogViewModel ViewModel => (DialogViewModel)(DataContext ?? throw new NullReferenceException());
}