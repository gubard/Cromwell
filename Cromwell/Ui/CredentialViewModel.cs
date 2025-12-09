using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class CredentialViewModel : ViewModelBase, IHeader
{
    private readonly IUiCredentialService _uiCredentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
    private readonly INavigator _navigator;
    private readonly INotificationService _notificationService;

    public CredentialViewModel(
        IUiCredentialService credentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        INavigator navigator,
        INotificationService notificationService,
        CredentialNotify credential
    )
    {
        _uiCredentialService = credentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _navigator = navigator;
        _notificationService = notificationService;
        Credential = credential;
        Header = new CredentialHeaderViewModel(credential);
    }

    public CredentialNotify Credential { get; }
    public object Header { get; }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await WrapCommand(() =>
            _uiCredentialService.GetAsync(new()
            {
                GetChildrenIds = [Credential.Id],
                GetParentsIds = [Credential.Id],
            }, ct)
        );
    }

    [RelayCommand]
    private async Task EditAsync(CredentialNotify credential,
        CancellationToken ct)
    {
        await WrapCommand(() =>
            _navigator.NavigateToAsync(
                new EditCredentialViewModel(credential, _uiCredentialService,
                    _notificationService,
                    _appResourceService), ct));
    }

    [RelayCommand]
    private async Task DeleteAsync(
        CredentialNotify parametersViewModel,
        CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            await _uiCredentialService.PostAsync(new()
            {
                DeleteIds = [parametersViewModel.Id],
            }, ct);
            await InitializedAsync(ct);
        });
    }

    [RelayCommand]
    private async Task ShowCreateViewAsync(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel();

        await WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _stringFormater.Format(
                _appResourceService.GetResource<string>("Lang.CreatingNewItem"),
                _appResourceService.GetResource<string>("Lang.Credential")),
            credential,
            new DialogButton(
                _appResourceService.GetResource<string>("Lang.Create"),
                CreateCommand,
                credential, DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task CreateAsync(
        CredentialParametersViewModel parametersViewModel,
        CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            parametersViewModel.StartExecute();

            if (parametersViewModel.HasErrors)
            {
                return;
            }

            await _uiCredentialService.PostAsync(new()
                {
                    CreateCredentials =
                    [
                        new()
                        {
                            Id = Guid.NewGuid(),
                            Name = parametersViewModel.Name,
                            Login = parametersViewModel.Login,
                            Key = parametersViewModel.Key,
                            IsAvailableUpperLatin = parametersViewModel
                               .IsAvailableUpperLatin,
                            IsAvailableLowerLatin = parametersViewModel
                               .IsAvailableLowerLatin,
                            IsAvailableNumber =
                                parametersViewModel.IsAvailableNumber,
                            IsAvailableSpecialSymbols = parametersViewModel
                               .IsAvailableSpecialSymbols,
                            CustomAvailableCharacters = parametersViewModel
                               .CustomAvailableCharacters,
                            Length = parametersViewModel.Length,
                            Regex = parametersViewModel.Regex,
                            Type = parametersViewModel.Type,
                            ParentId = Credential.Id,
                        },
                    ],
                }
                , ct);

            _dialogService.CloseMessageBox();
            await InitializedAsync(ct);
        });
    }
}