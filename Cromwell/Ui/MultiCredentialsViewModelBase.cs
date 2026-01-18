using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
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
    protected readonly AvaloniaList<CredentialNotify> _selectedCredentials;
    protected readonly IUiCredentialService UiCredentialService;
    protected readonly IDialogService DialogService;
    protected readonly IStringFormater StringFormater;
    protected readonly IAppResourceService AppResourceService;

    protected MultiCredentialsViewModelBase(
        IUiCredentialService uiCredentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService
    )
    {
        _selectedCredentials = new();
        UiCredentialService = uiCredentialService;
        DialogService = dialogService;
        StringFormater = stringFormater;
        AppResourceService = appResourceService;
    }

    public IEnumerable<CredentialNotify> SelectedCredentials => _selectedCredentials;

    [RelayCommand]
    private async Task MultiDeleteAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => MultiDeleteCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<TurtlePostResponse> MultiDeleteCore(CancellationToken ct)
    {
        var response = await UiCredentialService.PostAsync(
            Guid.NewGuid(),
            new() { DeleteIds = SelectedCredentials.Select(x => x.Id).ToArray() },
            ct
        );

        DialogService.DispatchCloseMessageBox();

        return response;
    }

    [RelayCommand]
    private async Task ShowMultiDelete(CancellationToken ct)
    {
        var header = Dispatcher.UIThread.Invoke(() =>
            AppResourceService.GetResource<string>("Lang.Delete").ToDialogHeader()
        );

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

        var header = Dispatcher.UIThread.Invoke(() =>
            StringFormater
                .Format(
                    AppResourceService.GetResource<string>("Lang.EditItem"),
                    SelectedCredentials.Select(x => x.Name).JoinString(", ")
                )
                .ToDialogHeader()
        );

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
            return new EmptyValidationErrors();
        }

        var editCredentials = parameters.CreateEditCredential();
        editCredentials.Ids = SelectedCredentials.Select(x => x.Id).ToArray();

        var response = await UiCredentialService.PostAsync(
            Guid.NewGuid(),
            new() { EditCredentials = [editCredentials] },
            ct
        );

        DialogService.DispatchCloseMessageBox();

        return response;
    }
}
