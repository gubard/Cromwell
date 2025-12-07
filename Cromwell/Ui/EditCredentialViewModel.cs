using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Services;

namespace Cromwell.Ui;

public partial class EditCredentialViewModel : ViewModelBase
{
    private readonly IUiCredentialService _uiCredentialService;
    private readonly INotificationService _notificationService;
    private readonly IAppResourceService _appResourceService;

    public EditCredentialViewModel(
        CredentialParametersViewModel credentialParameters,
        IUiCredentialService uiCredentialService,
        INotificationService notificationService,
        IAppResourceService appResourceService
    )
    {
        CredentialParameters = credentialParameters;
        _uiCredentialService = uiCredentialService;
        _notificationService = notificationService;
        _appResourceService = appResourceService;
        CredentialParameters.PropertyChanged += (_, e) => OnPropertyChanged(nameof(CanSave));
    }

    public CredentialParametersViewModel CredentialParameters { get; }

    public bool CanSave
    {
        get => CredentialParameters is { HasErrors: false, IsEdit: true };
    }

    [RelayCommand]
    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            CredentialParameters.StartExecute();

            if (CredentialParameters.HasErrors)
            {
                return;
            }

            await _uiCredentialService.PostAsync(new()
            {
                EditCredentials =
                [
                    new()
                    {
                        Id = CredentialParameters.Id,
                        CustomAvailableCharacters = CredentialParameters.CustomAvailableCharacters,
                        IsAvailableLowerLatin = CredentialParameters.IsAvailableLowerLatin,
                        IsAvailableNumber = CredentialParameters.IsAvailableNumber,
                        IsAvailableSpecialSymbols = CredentialParameters.IsAvailableSpecialSymbols,
                        IsAvailableUpperLatin = CredentialParameters.IsAvailableUpperLatin,
                        IsEditCustomAvailableCharacters = CredentialParameters.IsEditCustomAvailableCharacters,
                        IsEditIsAvailableLowerLatin = CredentialParameters.IsEditIsAvailableLowerLatin,
                        IsEditIsAvailableNumber = CredentialParameters.IsEditIsAvailableNumber,
                        IsEditIsAvailableSpecialSymbols = CredentialParameters.IsEditIsAvailableSpecialSymbols,
                        IsEditIsAvailableUpperLatin = CredentialParameters.IsEditIsAvailableUpperLatin,
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
            }, cancellationToken);

            _notificationService.ShowNotification(new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Saved"),
                Classes =
                {
                    "alignment-center",
                },
            }, NotificationType.None);
        });
    }
}