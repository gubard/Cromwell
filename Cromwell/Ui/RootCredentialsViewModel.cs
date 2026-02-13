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

public sealed partial class RootCredentialsViewModel
    : MultiCredentialsViewModelBase,
        IHeader,
        IRefresh,
        IInitUi,
        ISaveUi
{
    public RootCredentialsViewModel(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialUiCache credentialUiCache,
        ICromwellViewModelFactory factory
    )
        : base(credentialUiService, dialogService, stringFormater, appResourceService)
    {
        _credentialUiCache = credentialUiCache;

        Header = factory.CreateRootCredentialsHeader(
            new AvaloniaList<InannaCommand>
            {
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
            }
        );
    }

    public IEnumerable<CredentialNotify> Credentials => _credentialUiCache.Roots;
    public RootCredentialsHeaderViewModel Header { get; }
    object IHeader.Header => Header;

    public ConfiguredValueTaskAwaitable RefreshAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            () => CredentialUiService.GetAsync(new() { IsGetRoots = true }, ct),
            ct
        );
    }

    public ConfiguredValueTaskAwaitable SaveUiAsync(CancellationToken ct)
    {
        Selected.PropertyChanged -= SelectedCredentialsPropertyChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        Selected.PropertyChanged += SelectedCredentialsPropertyChanged;

        return RefreshAsync(ct);
    }

    private readonly ICredentialUiCache _credentialUiCache;

    private void SelectedCredentialsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Selected.Count))
        {
            return;
        }

        if (Selected.Count == 0)
        {
            foreach (var headerCommand in Header.AdaptiveButtons.Commands)
            {
                headerCommand.IsEnable = false;
            }
        }
        else
        {
            foreach (var headerCommand in Header.AdaptiveButtons.Commands)
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

        var credential = parameters.CreateCredential(Guid.NewGuid(), null);
        await DialogService.CloseMessageBoxAsync(ct);

        return await CredentialUiService.PostAsync(
            Guid.NewGuid(),
            new() { CreateCredentials = [credential] },
            ct
        );
        ;
    }
}
