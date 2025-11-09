using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Enums;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class EditCredentialViewModel : ViewModelBase
{
    private readonly ICredentialService _credentialService;
    private readonly INotificationService _notificationService;
    private readonly IApplicationResourceService _applicationResourceService;

    public EditCredentialViewModel(
        CredentialParametersViewModel credentialParameters,
        ICredentialService credentialService,
        INotificationService notificationService,
        IApplicationResourceService applicationResourceService
    )
    {
        CredentialParameters = credentialParameters;
        _credentialService = credentialService;
        _notificationService = notificationService;
        _applicationResourceService = applicationResourceService;
    }

    public CredentialParametersViewModel CredentialParameters { get; }

    [RelayCommand]
    private Task SaveAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            CredentialParameters.StartExecute();

            if (CredentialParameters.HasErrors)
            {
                return;
            }

            await _credentialService.EditAsync([
                new(CredentialParameters.Id)
                {
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
            ], cancellationToken);

            _notificationService.ShowNotification(_applicationResourceService.GetResource<string>("Lang.Saved"),
                NotificationType.None);
        });
    }
}