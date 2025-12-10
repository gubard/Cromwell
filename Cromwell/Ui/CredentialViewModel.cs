using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class CredentialViewModel : ViewModelBase, IHeader, IRefresh
{
    private readonly IUiCredentialService _uiCredentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;

    public CredentialViewModel(
        IUiCredentialService credentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        CredentialNotify credential
    )
    {
        _uiCredentialService = credentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        Credential = credential;
        Header = new CredentialHeaderViewModel(credential);
    }

    public CredentialNotify Credential { get; }
    public object Header { get; }

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
                            IsAvailableUpperLatin = parameters
                               .IsAvailableUpperLatin,
                            IsAvailableLowerLatin = parameters
                               .IsAvailableLowerLatin,
                            IsAvailableNumber =
                                parameters.IsAvailableNumber,
                            IsAvailableSpecialSymbols = parameters
                               .IsAvailableSpecialSymbols,
                            CustomAvailableCharacters = parameters
                               .CustomAvailableCharacters,
                            Length = parameters.Length,
                            Regex = parameters.Regex,
                            Type = parameters.Type,
                            ParentId = Credential.Id,
                        },
                    ],
                }
                , ct);

            _dialogService.CloseMessageBox();
        });
    }

    public async ValueTask RefreshAsync(CancellationToken ct)
    {
        await WrapCommand(() =>
            _uiCredentialService.GetAsync(new()
            {
                GetChildrenIds = [Credential.Id],
                GetParentsIds = [Credential.Id],
            }, ct)
        );
    }

    public void Refresh()
    {
        WrapCommand(() =>
            _uiCredentialService.Get(new()
            {
                GetChildrenIds = [Credential.Id],
                GetParentsIds = [Credential.Id],
            })
        );
    }
}