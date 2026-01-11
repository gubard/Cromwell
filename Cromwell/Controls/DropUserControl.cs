using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;

namespace Cromwell.Controls;

public class DropUserControl : UserControl
{
    public DropUserControl()
    {
        _uiCredentialService = DiHelper.ServiceProvider.GetService<IUiCredentialService>();
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private readonly ReadOnlyMemory<string> _dropTags = new[]
    {
        "DropRoot",
        "DropUp",
        "DropDown",
        "DropParent",
    };

    private readonly IUiCredentialService _uiCredentialService;

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
        var data = e
            .DataTransfer.Items[0]
            .TryGetRaw(e.DataTransfer.Items[0].Formats[0])
            .As<byte[]>();

        if (data is null)
        {
            return;
        }

        var id = new Guid(data);

        switch (tag)
        {
            case "DropRoot":
            {
                _uiCredentialService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        EditCredentials =
                        [
                            new()
                            {
                                Ids = [id],
                                IsEditParentId = true,
                                ParentId = null,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropParent":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialNotify>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiCredentialService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        EditCredentials =
                        [
                            new()
                            {
                                Ids = [id],
                                IsEditParentId = true,
                                ParentId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropUp":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialNotify>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiCredentialService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        ChangeOrders =
                        [
                            new()
                            {
                                InsertIds = [id],
                                IsAfter = false,
                                StartId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
            case "DropDown":
            {
                if (e.Source is not IDataContextProvider dataContextProvider)
                {
                    return;
                }

                var viewModel = dataContextProvider.DataContext.As<CredentialNotify>();

                if (viewModel is null)
                {
                    return;
                }

                if (viewModel.Id == id)
                {
                    return;
                }

                _uiCredentialService.PostAsync(
                    Guid.NewGuid(),
                    new()
                    {
                        ChangeOrders =
                        [
                            new()
                            {
                                InsertIds = [id],
                                IsAfter = true,
                                StartId = viewModel.Id,
                            },
                        ],
                    },
                    CancellationToken.None
                );

                break;
            }
        }
    }
}
