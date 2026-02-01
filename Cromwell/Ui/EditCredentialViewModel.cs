using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class EditCredentialViewModel : ViewModelBase, IHeader, IInitUi, ISaveUi
{
    private readonly ICredentialUiService _credentialUiService;
    private readonly INotificationService _notificationService;
    private readonly IAppResourceService _appResourceService;
    private readonly CredentialNotify _credential;
    private readonly IStringFormater _stringFormater;

    public EditCredentialViewModel(
        CredentialNotify credential,
        ICredentialUiService credentialUiService,
        INotificationService notificationService,
        IAppResourceService appResourceService,
        IStringFormater stringFormater
    )
    {
        _credential = credential;
        CredentialParameters = new(credential, ValidationMode.ValidateAll, true);
        _credentialUiService = credentialUiService;
        _notificationService = notificationService;
        _appResourceService = appResourceService;
        _stringFormater = stringFormater;
    }

    public CredentialParametersViewModel CredentialParameters { get; }
    public bool CanSave => CredentialParameters is { HasErrors: false, IsEdit: true };

    public object Header =>
        new TextBlock
        {
            Text = _stringFormater.Format(
                _appResourceService.GetResource<string>("Lang.EditItem"),
                _credential.Name
            ),
            Classes = { "h2", "align-left-center" },
        };

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        CredentialParameters.PropertyChanged += CredentialParametersPropertyChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable SaveUiAsync(CancellationToken ct)
    {
        CredentialParameters.PropertyChanged -= CredentialParametersPropertyChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }

    private void CredentialParametersPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(nameof(CanSave));
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => SaveCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask<IValidationErrors> SaveCore(CancellationToken ct)
    {
        CredentialParameters.StartExecute();

        if (CredentialParameters.HasErrors)
        {
            return new EmptyValidationErrors();
        }

        var response = await _credentialUiService.PostAsync(
            Guid.NewGuid(),
            new()
            {
                EditCredentials =
                [
                    new()
                    {
                        Ids = [_credential.Id],
                        CustomAvailableCharacters = CredentialParameters.CustomAvailableCharacters,
                        IsAvailableLowerLatin = CredentialParameters.IsAvailableLowerLatin,
                        IsAvailableNumber = CredentialParameters.IsAvailableNumber,
                        IsAvailableSpecialSymbols = CredentialParameters.IsAvailableSpecialSymbols,
                        IsAvailableUpperLatin = CredentialParameters.IsAvailableUpperLatin,
                        IsEditCustomAvailableCharacters =
                            CredentialParameters.IsEditCustomAvailableCharacters,
                        IsEditIsAvailableLowerLatin =
                            CredentialParameters.IsEditIsAvailableLowerLatin,
                        IsEditIsAvailableNumber = CredentialParameters.IsEditIsAvailableNumber,
                        IsEditIsAvailableSpecialSymbols =
                            CredentialParameters.IsEditIsAvailableSpecialSymbols,
                        IsEditIsAvailableUpperLatin =
                            CredentialParameters.IsEditIsAvailableUpperLatin,
                        IsEditKey = CredentialParameters.IsEditKey,
                        IsEditLength = CredentialParameters.IsEditLength,
                        IsEditLogin = CredentialParameters.IsEditLogin,
                        IsEditName = CredentialParameters.IsEditName,
                        IsEditRegex = CredentialParameters.IsEditRegex,
                        Login = CredentialParameters.Login,
                        IsEditType = CredentialParameters.IsEditType,
                        Key = CredentialParameters.Key,
                        Length = CredentialParameters.Length,
                        Name = CredentialParameters.Name,
                        Regex = CredentialParameters.Regex,
                        Type = CredentialParameters.Type,
                    },
                ],
            },
            ct
        );

        _notificationService.ShowNotification(
            new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Saved"),
                Classes = { "align-center", "h2", "m-5" },
            },
            NotificationType.None
        );

        return response;
    }
}
