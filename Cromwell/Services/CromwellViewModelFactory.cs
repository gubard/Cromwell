using Avalonia.Collections;
using Cromwell.Models;
using Cromwell.Ui;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

public interface ICromwellViewModelFactory
{
    CredentialViewModel CreateCredential(CredentialNotify credential);
    CredentialParametersViewModel CreateCredentialParameters(CredentialNotify credential);
    CredentialTreeViewModel CreateCredentialTree();
    ChangeParentCredentialViewModel CreateChangeParentCredential();
    RootCredentialsViewModel CreateRootCredentials();

    CredentialParametersViewModel CreateCredentialParameters(
        CredentialNotify item,
        ValidationMode validationMode,
        bool isShowEdit
    );
    CredentialParametersViewModel CreateCredentialParameters(
        ValidationMode validationMode,
        bool isShowEdit
    );

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
    public CromwellViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CredentialViewModel CreateCredential(CredentialNotify credential)
    {
        return new(
            _serviceProvider.GetService<ICredentialUiService>(),
            _serviceProvider.GetService<IDialogService>(),
            _serviceProvider.GetService<IStringFormater>(),
            _serviceProvider.GetService<IAppResourceService>(),
            credential,
            this,
            _serviceProvider.GetService<InannaCommands>(),
            _serviceProvider.GetService<CromwellCommands>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public CredentialParametersViewModel CreateCredentialParameters(CredentialNotify credential)
    {
        return new(
            credential,
            ValidationMode.ValidateAll,
            false,
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public CredentialTreeViewModel CreateCredentialTree()
    {
        return new(
            _serviceProvider.GetService<ICredentialUiCache>(),
            _serviceProvider.GetService<ICredentialUiService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public ChangeParentCredentialViewModel CreateChangeParentCredential()
    {
        return new(this, _serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public RootCredentialsViewModel CreateRootCredentials()
    {
        return new(
            _serviceProvider.GetService<ICredentialUiService>(),
            _serviceProvider.GetService<IDialogService>(),
            _serviceProvider.GetService<IStringFormater>(),
            _serviceProvider.GetService<IAppResourceService>(),
            _serviceProvider.GetService<ICredentialUiCache>(),
            this,
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<CromwellCommands>()
        );
    }

    public CredentialParametersViewModel CreateCredentialParameters(
        CredentialNotify item,
        ValidationMode validationMode,
        bool isShowEdit
    )
    {
        return new(
            item,
            validationMode,
            isShowEdit,
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public CredentialParametersViewModel CreateCredentialParameters(
        ValidationMode validationMode,
        bool isShowEdit
    )
    {
        return new(validationMode, isShowEdit, _serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public CredentialHeaderViewModel CreateCredentialHeader(
        CredentialNotify credential,
        IAvaloniaReadOnlyList<InannaCommand> multiCommands
    )
    {
        return new(
            credential,
            multiCommands,
            _serviceProvider.GetService<IInannaViewModelFactory>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public RootCredentialsHeaderViewModel CreateRootCredentialsHeader(
        IAvaloniaReadOnlyList<InannaCommand> commands
    )
    {
        return new(
            commands,
            _serviceProvider.GetService<IInannaViewModelFactory>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    private readonly IServiceProvider _serviceProvider;
}
