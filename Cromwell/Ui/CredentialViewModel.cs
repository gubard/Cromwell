using System.ComponentModel;
using System.Runtime.CompilerServices;
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

public sealed partial class CredentialViewModel
    : MultiCredentialsViewModelBase,
        IHeader,
        IRefresh,
        IInitUi,
        ISaveUi
{
    public CredentialViewModel(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        CredentialNotify credential
    )
        : base(credentialUiService, dialogService, stringFormater, appResourceService)
    {
        Credential = credential;
        Header = new(
            credential,
            [
                new(
                    ShowMultiEditCommand,
                    null,
                    appResourceService.GetResource<string>("Lang.Edit"),
                    PackIconMaterialDesignKind.Edit,
                    isEnable: false
                ),
                new(
                    ShowMultiDeleteCommand,
                    null,
                    appResourceService.GetResource<string>("Lang.Delete"),
                    PackIconMaterialDesignKind.Delete,
                    ButtonType.Danger,
                    false
                ),
            ]
        );
    }

    public CredentialNotify Credential { get; }
    public CredentialHeaderViewModel Header { get; }
    object IHeader.Header => Header;

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        _selectedCredentials.PropertyChanged += SelectedCredentialsPropertyChanged;

        return RefreshAsync(ct);
    }

    public ConfiguredValueTaskAwaitable RefreshAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            () =>
                CredentialUiService.GetAsync(
                    new() { GetChildrenIds = [Credential.Id], GetParentsIds = [Credential.Id] },
                    ct
                ),
            ct
        );
    }

    private void SelectedCredentialsPropertyChanged(object? sender, PropertyChangedEventArgs e)
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
    }

    [RelayCommand]
    private async Task ShowCreateViewAsync(CancellationToken ct)
    {
        var credential = new CredentialParametersViewModel(ValidationMode.ValidateAll, false);

        await WrapCommandAsync(
            () =>
                DialogService.ShowMessageBoxAsync(
                    new(
                        StringFormater
                            .Format(
                                AppResourceService.GetResource<string>("Lang.CreatingNewItem"),
                                AppResourceService.GetResource<string>("Lang.Credential")
                            )
                            .DispatchToDialogHeader(),
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
            return new DefaultValidationErrors();
        }

        var response = await CredentialUiService.PostAsync(
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
                        ParentId = Credential.Id,
                    },
                ],
            },
            ct
        );

        await DialogService.CloseMessageBoxAsync(ct);

        return response;
    }

    public ConfiguredValueTaskAwaitable SaveUiAsync(CancellationToken ct)
    {
        _selectedCredentials.PropertyChanged -= SelectedCredentialsPropertyChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }
}
