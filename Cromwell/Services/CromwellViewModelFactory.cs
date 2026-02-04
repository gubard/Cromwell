using Cromwell.Models;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Services;

namespace Cromwell.Services;

public interface ICromwellViewModelFactory
{
    CredentialViewModel CreateCredential(CredentialNotify credential);
    EditCredentialViewModel EditCredential(CredentialNotify credential);
}

public sealed class CromwellViewModelFactory : ICromwellViewModelFactory
{
    public CromwellViewModelFactory(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        INotificationService notificationService
    )
    {
        _credentialUiService = credentialUiService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _notificationService = notificationService;
    }

    public CredentialViewModel CreateCredential(CredentialNotify credential)
    {
        return new(
            _credentialUiService,
            _dialogService,
            _stringFormater,
            _appResourceService,
            credential
        );
    }

    public EditCredentialViewModel EditCredential(CredentialNotify credential)
    {
        return new(
            credential,
            _credentialUiService,
            _notificationService,
            _appResourceService,
            _stringFormater
        );
    }

    private readonly ICredentialUiService _credentialUiService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
    private readonly INotificationService _notificationService;
}
