using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public sealed partial class RootCredentialsViewModel
    : MultiCredentialsViewModelBase,
        IHeader,
        IRefresh
{
    private readonly ICredentialCache _credentialCache;

    public RootCredentialsViewModel(
        IUiCredentialService uiCredentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialCache credentialCache
    )
        : base(uiCredentialService, dialogService, stringFormater, appResourceService)
    {
        _credentialCache = credentialCache;

        Header = new([
            new(
                ShowMultiEditCommand,
                null,
                new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.Edit },
                isEnable: false
            ),
            new(
                ShowMultiDeleteCommand,
                null,
                new PackIconMaterialDesign { Kind = PackIconMaterialDesignKind.Delete },
                ButtonType.Danger,
                false
            ),
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
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateAll, false);

        await WrapCommandAsync(
            () =>
                DialogService.ShowMessageBoxAsync(
                    new(
                        StringFormater.Format(
                            AppResourceService.GetResource<string>("Lang.CreatingNewItem"),
                            AppResourceService.GetResource<string>("Lang.Credential")
                        ),
                        credential,
                        new DialogButton(
                            AppResourceService.GetResource<string>("Lang.Create"),
                            CreateCommand,
                            credential,
                            DialogButtonType.Primary
                        ),
                        UiHelper.CancelButton
                    ),
                    ct
                ),
            ct
        );
    }

    [RelayCommand]
    private async Task CreateAsync(CredentialParametersViewModel parameters, CancellationToken ct)
    {
        await WrapCommandAsync(
            async () =>
            {
                parameters.StartExecute();

                if (parameters.HasErrors)
                {
                    return (IValidationErrors)EmptyValidationErrors.Instance;
                }

                var response = await UiCredentialService.PostAsync(
                    new()
                    {
                        CreateCredentials =
                        [
                            new()
                            {
                                Id = Guid.NewGuid(),
                                Name = parameters.Name,
                                Login = parameters.Login,
                                Key = parameters.Key,
                                IsAvailableUpperLatin = parameters.IsAvailableUpperLatin,
                                IsAvailableLowerLatin = parameters.IsAvailableLowerLatin,
                                IsAvailableNumber = parameters.IsAvailableNumber,
                                IsAvailableSpecialSymbols = parameters.IsAvailableSpecialSymbols,
                                CustomAvailableCharacters = parameters.CustomAvailableCharacters,
                                Length = parameters.Length,
                                Regex = parameters.Regex,
                                Type = parameters.Type,
                            },
                        ],
                    },
                    ct
                );

                DialogService.CloseMessageBox();

                return response;
            },
            ct
        );
    }

    public ValueTask RefreshAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            () => UiCredentialService.GetAsync(new() { IsGetRoots = true }, ct),
            ct
        );
    }

    public void Refresh()
    {
        WrapCommand(() => UiCredentialService.Get(new() { IsGetRoots = true }));
    }
}
