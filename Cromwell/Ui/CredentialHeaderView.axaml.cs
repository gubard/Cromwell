using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Cromwell.Services;
using Gaia.Helpers;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class CredentialHeaderView : UserControl
{
    private readonly ReadOnlyMemory<string> _dropTags = new[]
    {
        "DropRoot",
        "DropUp",
        "DropDown",
        "DropParent",
    };

    private readonly ICredentialService _credentialService;
    private readonly INavigator _navigator;

    public CredentialHeaderView()
    {
        InitializeComponent();
        _credentialService = DiHelper.ServiceProvider.GetService<ICredentialService>();
        _navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    public CredentialHeaderViewModel ViewModel =>
        (CredentialHeaderViewModel)(DataContext ?? throw new NullReferenceException());

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

        (_navigator.CurrentView as CredentialViewModel)?.InitializedCommand.Execute(null);
    }
}