using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        CredentialNotify credential,
        ICromwellViewModelFactory factory
    )
        : base(credentialUiService, dialogService, stringFormater, appResourceService)
    {
        Credential = credential;

        Header = factory.CreateCredentialHeader(
            credential,
            new AvaloniaList<InannaCommand>
            {
                new(
                    ShowMultiEditCommand,
                    null,
                    appResourceService.GetResource<string>("Lang.Edit"),
                    PackIconMaterialDesignKind.Edit
                ),
                new(
                    ShowMultiDeleteCommand,
                    null,
                    appResourceService.GetResource<string>("Lang.Delete"),
                    PackIconMaterialDesignKind.Delete,
                    ButtonType.Danger
                ),
            }
        );
    }

    public CredentialNotify Credential { get; }
    public CredentialHeaderViewModel Header { get; }
    object IHeader.Header => Header;

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        Selected.PropertyChanged += SelectedCredentialsPropertyChanged;

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
        if (e.PropertyName != nameof(Selected.Count))
        {
            return;
        }

        if (Selected.Count == 0)
        {
            foreach (var headerCommand in Header.MultiAdaptiveButtons.Commands)
            {
                headerCommand.IsEnable = false;
            }
        }
        else
        {
            foreach (var headerCommand in Header.MultiAdaptiveButtons.Commands)
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

        var credential = parameters.CreateCredential(Guid.NewGuid(), Credential.Id);
        await DialogService.CloseMessageBoxAsync(ct);

        return await CredentialUiService.PostAsync(
            Guid.NewGuid(),
            new() { CreateCredentials = [credential] },
            ct
        );
        ;
    }

    public ConfiguredValueTaskAwaitable SaveUiAsync(CancellationToken ct)
    {
        Selected.PropertyChanged -= SelectedCredentialsPropertyChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }
}
