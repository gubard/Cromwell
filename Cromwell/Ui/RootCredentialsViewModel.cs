using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class RootCredentialsViewModel : ViewModelBase, IHeader, IRefresh
{
    private readonly IUiCredentialService _uiCredentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
    private readonly ICredentialCache _credentialCache;

    public RootCredentialsViewModel(
        IUiCredentialService uiCredentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialCache credentialCache)
    {
        _uiCredentialService = uiCredentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _credentialCache = credentialCache;
    }

    public IEnumerable<CredentialNotify> Credentials
    {
        get => _credentialCache.Roots;
    }

    public object Header
    {
        get => new TextBlock
        {
            Text = _appResourceService.GetResource<string>("Lang.Credentials"),
            Classes =
            {
                "h2",
            },
        };
    }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await RefreshAsync(ct);
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
                        IsAvailableUpperLatin =
                            parametersViewModel.IsAvailableUpperLatin,
                        IsAvailableLowerLatin =
                            parametersViewModel.IsAvailableLowerLatin,
                        IsAvailableNumber =
                            parametersViewModel.IsAvailableNumber,
                        IsAvailableSpecialSymbols = parametersViewModel
                           .IsAvailableSpecialSymbols,
                        CustomAvailableCharacters = parametersViewModel
                           .CustomAvailableCharacters,
                        Length = parametersViewModel.Length,
                        Regex = parametersViewModel.Regex,
                        Type = parametersViewModel.Type,
                    },
                ],
            }, ct);

            _dialogService.CloseMessageBox();
        });
    }

    public async ValueTask RefreshAsync(CancellationToken ct)
    {
        await WrapCommand(async () =>
            await _uiCredentialService.GetAsync(new()
            {
                IsGetRoots = true,
            }, ct));
    }

    public void Refresh()
    {
        WrapCommand(() =>
            _uiCredentialService.Get(new()
            {
                IsGetRoots = true,
            }));
    }
}