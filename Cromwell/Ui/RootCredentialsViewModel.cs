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
        ICredentialCache credentialCache,
        RootCredentialsHeaderViewModel header)
    {
        _uiCredentialService = uiCredentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _credentialCache = credentialCache;
        Header = header;
    }

    public IEnumerable<CredentialNotify> Credentials => _credentialCache.Roots;
    public RootCredentialsHeaderViewModel Header { get; }
    object IHeader.Header => Header;

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
        CredentialParametersViewModel parameters,
        CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            parameters.StartExecute();

            if (parameters.HasErrors)
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
                        Name = parameters.Name,
                        Login = parameters.Login,
                        Key = parameters.Key,
                        IsAvailableUpperLatin =
                            parameters.IsAvailableUpperLatin,
                        IsAvailableLowerLatin =
                            parameters.IsAvailableLowerLatin,
                        IsAvailableNumber =
                            parameters.IsAvailableNumber,
                        IsAvailableSpecialSymbols = parameters
                           .IsAvailableSpecialSymbols,
                        CustomAvailableCharacters = parameters
                           .CustomAvailableCharacters,
                        Length = parameters.Length,
                        Regex = parameters.Regex,
                        Type = parameters.Type,
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