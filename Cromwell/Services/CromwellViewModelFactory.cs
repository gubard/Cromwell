using Avalonia.Collections;
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
    ChangeParentCredentialViewModel CreateChangeParentCredential();

    CredentialHeaderViewModel CreateCredentialHeader(
        CredentialNotify credential,
        IAvaloniaReadOnlyList<InannaCommand> multiCommands
    );

    RootCredentialsHeaderViewModel CreateRootCredentialsHeader(
        IAvaloniaReadOnlyList<InannaCommand> commands
    );
}

public sealed class CromwellViewModelFactory : ICromwellViewModelFactory
{
    public CromwellViewModelFactory(
        ICredentialUiService credentialUiService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IAppResourceService appResourceService,
        ICredentialUiCache credentialUiCache,
        IInannaViewModelFactory inannaViewModelFactor
    )
    {
        _credentialUiService = credentialUiService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _credentialUiCache = credentialUiCache;
        _inannaViewModelFactor = inannaViewModelFactor;
    }

    public CredentialViewModel CreateCredential(CredentialNotify credential)
    {
        return new(
            _credentialUiService,
            _dialogService,
            _stringFormater,
            _appResourceService,
            credential,
            this
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

    public ChangeParentCredentialViewModel CreateChangeParentCredential()
    {
        return new(this);
    }

    public CredentialHeaderViewModel CreateCredentialHeader(
        CredentialNotify credential,
        IAvaloniaReadOnlyList<InannaCommand> multiCommands
    )
    {
        return new(credential, multiCommands, _inannaViewModelFactor);
    }

    public RootCredentialsHeaderViewModel CreateRootCredentialsHeader(
        IAvaloniaReadOnlyList<InannaCommand> commands
    )
    {
        return new(commands, _inannaViewModelFactor);
    }

    private readonly ICredentialUiCache _credentialUiCache;
    private readonly ICredentialUiService _credentialUiService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IAppResourceService _appResourceService;
    private readonly IInannaViewModelFactory _inannaViewModelFactor;
}
