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
    CredentialTreeViewModel CreateCredentialTree();
    ChangeParentCredentialViewModel ChangeParentCredential();
}

public sealed class CromwellViewModelFactory : ICromwellViewModelFactory
{
    public CromwellViewModelFactory(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialUiCache credentialUiCache
    )
    {
        _credentialUiService = credentialUiService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _credentialUiCache = credentialUiCache;
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

    public CredentialTreeViewModel CreateCredentialTree()
    {
        return new(_credentialUiCache, _credentialUiService);
    }

    public ChangeParentCredentialViewModel ChangeParentCredential()
    {
        return new(this);
    }

    private readonly ICredentialUiCache _credentialUiCache;
    private readonly ICredentialUiService _credentialUiService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
}
