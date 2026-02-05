using Avalonia.Collections;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;

namespace Cromwell.Ui;

public abstract partial class MultiCredentialsViewModelBase : ViewModelBase
{
    protected readonly AvaloniaList<CredentialNotify> Selected;
    protected readonly ICredentialUiService CredentialUiService;
    protected readonly IDialogService DialogService;
    protected readonly IStringFormater StringFormater;
    protected readonly IAppResourceService AppResourceService;

    protected MultiCredentialsViewModelBase(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService
    )
    {
        Selected = new();
        CredentialUiService = credentialUiService;
        DialogService = dialogService;
        StringFormater = stringFormater;
        AppResourceService = appResourceService;
    }

    public IEnumerable<CredentialNotify> SelectedCredentials => Selected;

    [RelayCommand]
    private async Task MultiDeleteAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => MultiDeleteCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<TurtlePostResponse> MultiDeleteCore(CancellationToken ct)
    {
        var response = await CredentialUiService.PostAsync(
            Guid.NewGuid(),
            new() { DeleteIds = SelectedCredentials.Select(x => x.Id).ToArray() },
            ct
        );

        await DialogService.CloseMessageBoxAsync(ct);

        return response;
    }

    [RelayCommand]
    private async Task ShowMultiDelete(CancellationToken ct)
    {
        var header = AppResourceService.GetResource<string>("Lang.Delete").DispatchToDialogHeader();

        var button = new DialogButton(
            AppResourceService.GetResource<string>("Lang.Delete"),
            MultiDeleteCommand,
            null,
            DialogButtonType.Primary
        );

        await WrapCommandAsync(
            () =>
                DialogService.ShowMessageBoxAsync(
                    new(
                        header,
                        new TextBlock
                        {
                            Text = StringFormater.Format(
                                AppResourceService.GetResource<string>("Lang.DeleteAsk"),
                                SelectedCredentials.Select(x => x.Name).JoinString(", ")
                            ),
                        },
                        button,
                        UiHelper.CancelButton
                    ),
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private async Task ShowMultiEdit(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateOnlyEdited, true);

        var header = StringFormater
            .Format(
                AppResourceService.GetResource<string>("Lang.EditItem"),
                SelectedCredentials.Select(x => x.Name).JoinString(", ")
            )
            .DispatchToDialogHeader();

        var button = new DialogButton(
            AppResourceService.GetResource<string>("Lang.Edit"),
            MultiEditCommand,
            (SelectedCredentials, credential),
            DialogButtonType.Primary
        );

        await WrapCommandAsync(
            () =>
                DialogService.ShowMessageBoxAsync(
                    new(header, credential, button, UiHelper.CancelButton),
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private async Task MultiEditAsync(
        CredentialParametersViewModel parameters,
        CancellationToken ct
    )
    {
        await WrapCommandAsync(() => MultiEditCore(parameters, ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<IValidationErrors> MultiEditCore(
        CredentialParametersViewModel parameters,
        CancellationToken ct
    )
    {
        if (parameters.HasErrors)
        {
            return new DefaultValidationErrors();
        }

        var ids = SelectedCredentials.Select(x => x.Id).ToArray();
        var editCredentials = parameters.CreateEditCredential(ids);

        var response = await CredentialUiService.PostAsync(
            Guid.NewGuid(),
            new() { Edits = [editCredentials] },
            ct
        );

        await DialogService.CloseMessageBoxAsync(ct);

        return response;
    }
}
