using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
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
    private readonly AvaloniaList<CredentialNotify> _selectedCredentials;

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
        _selectedCredentials = [];

        Header = new([
            new(ShowMultiEditCommand, null, new PackIconMaterialDesign
            {
                Kind = PackIconMaterialDesignKind.Edit,
            }, false),
        ]);

        _selectedCredentials.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(_selectedCredentials.Count))
            {
                return;
            }

            if (_selectedCredentials.Count == 0)
            {
                foreach (var headerCommand in Header.Commands)
                {
                    headerCommand.IsEnable = false;
                }
            }
            else
            {
                foreach (var headerCommand in Header.Commands)
                {
                    headerCommand.IsEnable = true;
                }
            }
        };
    }

    public IEnumerable<CredentialNotify> Credentials => _credentialCache.Roots;
    public IEnumerable<CredentialNotify> SelectedCredentials => _selectedCredentials;
    public RootCredentialsHeaderViewModel Header { get; }
    object IHeader.Header => Header;

    [RelayCommand]
    private async Task ShowMultiEdit(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateOnlyEdited, true);

        var header = _stringFormater.Format(
            _appResourceService.GetResource<string>("Lang.EditItem"),
            SelectedCredentials.Select(x => x.Name).JoinString(", "));

        var button = new DialogButton(
            _appResourceService.GetResource<string>("Lang.Edit"),
            MultiEditCommand, (SelectedCredentials, credential), DialogButtonType.Primary);

        await WrapCommand(() =>
            _dialogService.ShowMessageBoxAsync(new(header, credential, button, UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task MultiEdit(
        (IEnumerable<CredentialNotify> credentials, CredentialParametersViewModel parameters) value,
        CancellationToken ct)
    {
        await WrapCommand(async () =>
        {
            var (credentials, parameters) = value;
            parameters.StartExecute();

            if (parameters.HasErrors)
            {
                return (IValidationErrors)EmptyValidationErrors.Instance;
            }

            var editCredentials = parameters.CreateEditCredential();
            editCredentials.Ids = credentials.Select(x => x.Id).ToArray();
            var response = await _uiCredentialService.PostAsync(new()
            {
                EditCredentials =
                [
                    editCredentials,
                ],
            }, ct);

            _dialogService.CloseMessageBox();

            return response;
        });
    }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken ct)
    {
        await RefreshAsync(ct);
    }

    [RelayCommand]
    private async Task ShowCreateViewAsync(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateAll, false);

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
                return (IValidationErrors)EmptyValidationErrors.Instance;
            }

            var response = await _uiCredentialService.PostAsync(new()
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

            return response;
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