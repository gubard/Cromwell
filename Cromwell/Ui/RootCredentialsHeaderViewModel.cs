using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Cromwell.Ui;

public partial class RootCredentialsHeaderViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isMulti;
}