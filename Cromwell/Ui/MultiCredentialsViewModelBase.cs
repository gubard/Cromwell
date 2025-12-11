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

namespace Cromwell.Ui;

public abstract partial class MultiCredentialsViewModelBase : ViewModelBase
{
    protected readonly AvaloniaList<CredentialNotify> _selectedCredentials;
    protected readonly IUiCredentialService UiCredentialService;
    protected readonly IDialogService DialogService;
    protected readonly IStringFormater StringFormater;
    protected readonly IAppResourceService AppResourceService;

    protected MultiCredentialsViewModelBase(IUiCredentialService uiCredentialService, IDialogService dialogService,
        IStringFormater stringFormater, IAppResourceService appResourceService)
    {
        _selectedCredentials = new();
        UiCredentialService = uiCredentialService;
        DialogService = dialogService;
        StringFormater = stringFormater;
        AppResourceService = appResourceService;
    }

    public IEnumerable<CredentialNotify> SelectedCredentials => _selectedCredentials;

    [RelayCommand]
    private async Task MultiDelete(CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            await UiCredentialService.PostAsync(new()
            {
                DeleteIds = SelectedCredentials.Select(x => x.Id).ToArray(),
            }, ct);

            DialogService.CloseMessageBox();
        });
    }

    [RelayCommand]
    private async Task ShowMultiDelete(CancellationToken ct)
    {
        var header = AppResourceService.GetResource<string>("Lang.Delete");

        var button = new DialogButton(
            AppResourceService.GetResource<string>("Lang.Delete"),
            MultiDeleteCommand, null, DialogButtonType.Primary);

        await WrapCommand(() =>
            DialogService.ShowMessageBoxAsync(new(header, new TextBlock
            {
                Text = StringFormater.Format(
                    AppResourceService.GetResource<string>("Lang.DeleteAsk"),
                    SelectedCredentials.Select(x => x.Name).JoinString(", ")),
            }, button, UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task ShowMultiEdit(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateOnlyEdited, true);

        var header = StringFormater.Format(
            AppResourceService.GetResource<string>("Lang.EditItem"),
            SelectedCredentials.Select(x => x.Name).JoinString(", "));

        var button = new DialogButton(
            AppResourceService.GetResource<string>("Lang.Edit"),
            MultiEditCommand, (SelectedCredentials, credential), DialogButtonType.Primary);

        await WrapCommand(() =>
            DialogService.ShowMessageBoxAsync(new(header, credential, button, UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task MultiEdit(CredentialParametersViewModel parameters, CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            parameters.StartExecute();

            if (parameters.HasErrors)
            {
                return (IValidationErrors)EmptyValidationErrors.Instance;
            }

            var editCredentials = parameters.CreateEditCredential();
            editCredentials.Ids = SelectedCredentials.Select(x => x.Id).ToArray();
            var response = await UiCredentialService.PostAsync(new()
            {
                EditCredentials =
                [
                    editCredentials,
                ],
            }, ct);

            DialogService.CloseMessageBox();

            return response;
        });
    }
}