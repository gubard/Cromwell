using System.Runtime.CompilerServices;
using Avalonia.Threading;
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
        IRefresh,
        IInitUi
{
    public RootCredentialsViewModel(
        IUiCredentialService uiCredentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialMemoryCache credentialMemoryCache
    )
        : base(uiCredentialService, dialogService, stringFormater, appResourceService)
    {
        _credentialMemoryCache = credentialMemoryCache;

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

    public IEnumerable<CredentialNotify> Credentials => _credentialMemoryCache.Roots;
    public RootCredentialsHeaderViewModel Header { get; }
    object IHeader.Header => Header;

    public ConfiguredValueTaskAwaitable RefreshAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            () => UiCredentialService.GetAsync(new() { IsGetRoots = true }, ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return RefreshAsync(ct);
    }

    private readonly ICredentialMemoryCache _credentialMemoryCache;

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
        await WrapCommandAsync(() => CreateCore(parameters, ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<IValidationErrors> CreateCore(
        CredentialParametersViewModel parameters,
        CancellationToken ct
    )
    {
        parameters.StartExecute();

        if (parameters.HasErrors)
        {
            return new EmptyValidationErrors();
        }

        var response = await UiCredentialService.PostAsync(
            Guid.NewGuid(),
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

        Dispatcher.UIThread.Post(() => DialogService.CloseMessageBox());

        return response;
    }
}
