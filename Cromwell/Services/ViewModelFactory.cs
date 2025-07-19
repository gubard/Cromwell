using Cromwell.Db;
using Cromwell.Ui;

namespace Cromwell.Services;

public interface IViewModelFactory
{
    EditCredentialViewModel CreateEditCredentialViewModel(Guid id);
    EditCredentialViewModel CreateEditCredentialViewModel(CredentialEntity entity);
}

public class ViewModelFactory : IViewModelFactory
{
    private readonly ICredentialService _credentialService;
    private readonly IDialogService _dialogService;

    public ViewModelFactory(ICredentialService credentialService, IDialogService dialogService)
    {
        _credentialService = credentialService;
        _dialogService = dialogService;
    }

    public EditCredentialViewModel CreateEditCredentialViewModel(Guid id)
    {
        return new(id, _credentialService, _dialogService);
    }

    public EditCredentialViewModel CreateEditCredentialViewModel(CredentialEntity entity)
    {
        return new(entity, _credentialService, _dialogService);
    }
}