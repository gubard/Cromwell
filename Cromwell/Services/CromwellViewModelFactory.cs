using Cromwell.Models;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Services;

public interface ICromwellViewModelFactory
{
    CredentialViewModel CreateCredential(CredentialNotify credential);
    CredentialParametersViewModel CreateCredentialParameters(CredentialNotify credential);
}

public sealed class CromwellViewModelFactory : ICromwellViewModelFactory
{
    public CromwellViewModelFactory(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService
    )
    {
        _credentialUiService = credentialUiService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
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

    public CredentialParametersViewModel CreateCredentialParameters(CredentialNotify credential)
    {
        return new(credential, ValidationMode.ValidateAll, false);
    }

    private readonly ICredentialUiService _credentialUiService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
}
