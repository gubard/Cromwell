using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Cromwell.Services;
using Gaia.Extensions;
using Inanna.Helpers;

namespace Cromwell.Ui;

public partial class RootCredentialsView : UserControl
{
private readonly ReadOnlyMemory<string> _dropTags = new[]
    {
        "DropRoot",
        "DropUp",
        "DropDown",
        "DropParent",
    };

    private readonly ICredentialService _credentialService;

    public RootCredentialsView()
    {
        InitializeComponent();
        _credentialService = DiHelper.ServiceProvider.GetService<ICredentialService>();
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    public RootCredentialsViewModel ViewModel =>
        (RootCredentialsViewModel)(DataContext ?? throw new NullReferenceException());

    private void DragOver(object? sender, DragEventArgs e)
    {
        var tag = FindObjectDropTag(e.Source);

        if (tag is not null && _dropTags.Span.Contains(tag))
        {
            e.DragEffects &= DragDropEffects.Move;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private string? FindObjectDropTag(object? obj)
    {
        if (obj is null)
        {
            return null;
        }

        if (obj is Panel panel)
        {
            return panel.Tag?.ToString();
        }

        return obj.As<Visual>()?.GetVisualParent<Panel>()?.Tag?.ToString();
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        var tag = FindObjectDropTag(e.Source);
        var data = e.DataTransfer.Items[0].TryGetRaw(e.DataTransfer.Items[0].Formats[0]).As<byte[]>();

        if (data is null)
        {
            return;
        }

        var id = new Guid(data);

        switch (tag)
        {
            case "DropRoot":
            {
                _credentialService.EditAsync([
                        new(id)
                        {
                            IsEditParentId = true,
                            ParentId = null,
                        },
                    ], CancellationToken.None)
                   .GetAwaiter()
                   .GetResult();

                break;
            }
            case "DropParent":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialParametersViewModel>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _credentialService.EditAsync([
                        new(id)
                        {
                            IsEditParentId = true,
                            ParentId = viewModel.Id,
                        },
                    ], CancellationToken.None)
                   .GetAwaiter()
                   .GetResult();

                break;
            }
            case "DropUp":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialParametersViewModel>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _credentialService.ChangeOrderAsync(viewModel.Id, [id,], false, CancellationToken.None)
                   .GetAwaiter()
                   .GetResult();

                break;
            }
            case "DropDown":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialParametersViewModel>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _credentialService.ChangeOrderAsync(viewModel.Id, [id,], true, CancellationToken.None)
                   .GetAwaiter()
                   .GetResult();

                break;
            }
        }

        ViewModel.InitializedCommand.Execute(null);
    }

    private async void RectangleOnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not IDataContextProvider dataContextProvider)
        {
            return;
        }

        if (dataContextProvider.DataContext is not CredentialParametersViewModel credentialParametersViewModel)
        {
            return;
        }

        e.Handled = true;
        var dragData = new DataTransfer();
        var dataTransferItem = new DataTransferItem();

        dataTransferItem.Set(DataFormat.CreateBytesApplicationFormat(nameof(CredentialParametersViewModel)),
            credentialParametersViewModel.Id.ToByteArray());

        dragData.Add(dataTransferItem);
        var item = sender.As<ILogical>()?.GetLogicalAncestors().OfType<TreeViewItem>().FirstOrDefault().As<Visual>();

        if (item is null)
        {
            await TopLevelAssist.DoDragDropAsync(e, dragData, DragDropEffects.Move);
        }
        else
        {
            item.IsVisible = false;
            await TopLevelAssist.DoDragDropAsync(e, dragData, DragDropEffects.Move);
            item.IsVisible = true;
        }
    }
}