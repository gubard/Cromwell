using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class CredentialsTreeView : UserControl
{
    private readonly IDragAndDropService _dragAndDropService;
    private readonly ICredentialService _credentialService;

    public CredentialsTreeView()
    {
        InitializeComponent();
        _dragAndDropService = DiHelper.ServiceProvider.GetService<IDragAndDropService>();
        _credentialService = DiHelper.ServiceProvider.GetService<ICredentialService>();
    }
    
    public CredentialsTreeViewModel ViewModel => (CredentialsTreeViewModel)(DataContext ?? throw new NullReferenceException());

    public async void RootButtonOnPointerReleased(object? sender, PointerEventArgs e)
    {
        if (!_dragAndDropService.IsDragging)
        {
            return;
        }

        if (_dragAndDropService.GetDataAndRelease() is not CredentialParametersViewModel data)
        {
            return;
        }

        await _credentialService.ChangeParentAsync(data.Id, null, CancellationToken.None);
        ViewModel.InitializedCommand.Execute(null);
    }

    public void TreeViewItemOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_dragAndDropService.IsDragging)
        {
            return;
        }

        if (e.Source is not Control control)
        {
            return;
        }

        var treeViewItem = control.FindAncestorOfType<TreeViewItem>();

        if (treeViewItem is null)
        {
            return;
        }

        if (treeViewItem.DataContext is not CredentialParametersViewModel viewModel)
        {
            return;
        }

        _dragAndDropService.SetData(viewModel);
    }

    public async void TreeViewItemOnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_dragAndDropService.IsDragging)
        {
            return;
        }

        if (sender is not Visual visual)
        {
            return;
        }

        var position = e.GetPosition(visual);
        var overControl = visual.GetVisualsAt(position).FirstOrDefault();

        if (overControl is null)
        {
            return;
        }

        var treeViewItem = overControl.FindAncestorOfType<TreeViewItem>();

        if (treeViewItem is null)
        {
            return;
        }

        if (treeViewItem.DataContext is not CredentialParametersViewModel viewModel)
        {
            return;
        }

        if (_dragAndDropService.GetDataAndRelease() is not CredentialParametersViewModel data)
        {
            return;
        }

        if (viewModel == data)
        {
            return;
        }

        await _credentialService.ChangeParentAsync(data.Id, viewModel.Id, CancellationToken.None);
        ViewModel.InitializedCommand.Execute(null);
    }
}