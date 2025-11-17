using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Db;
using Cromwell.Services;
using Gaia.Extensions;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public partial class RootCredentialsViewModel : ViewModelBase
{
    private readonly ICredentialService _credentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IApplicationResourceService _applicationResourceService;
    private readonly INavigator _navigator;
    private readonly INotificationService _notificationService;

    public RootCredentialsViewModel(
        ICredentialService credentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IApplicationResourceService applicationResourceService,
        INavigator navigator,
        INotificationService notificationService
    )
    {
        _credentialService = credentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _applicationResourceService = applicationResourceService;
        _navigator = navigator;
        _notificationService = notificationService;
    }

    public AvaloniaList<CredentialParametersViewModel> Credentials { get; } = new();

    [RelayCommand]
    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            Credentials.Clear();
            var credentials = await _credentialService.GetRootsAsync(cancellationToken);

            Credentials.AddRange(credentials.OrderBy(x => x.OrderIndex)
               .Select(x => new CredentialParametersViewModel(x)));
        });
    }

    [RelayCommand]
    private Task EditAsync(CredentialParametersViewModel credential, CancellationToken cancellationToken)
    {
        return WrapCommand(() =>
            _navigator.NavigateToAsync(
                new EditCredentialViewModel(credential, _credentialService, _notificationService,
                    _applicationResourceService), cancellationToken));
    }

    [RelayCommand]
    private Task DeleteAsync(CredentialParametersViewModel parametersViewModel, CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            await _credentialService.DeleteAsync(parametersViewModel.Id, cancellationToken);
            await InitializedAsync(cancellationToken);
        });
    }

    [RelayCommand]
    private Task ShowCreateViewAsync(CancellationToken cancellationToken)
    {
        var credential = new CredentialParametersViewModel(Guid.CreateVersion7());

        return WrapCommand(async () =>
        {
            await _dialogService.ShowMessageBoxAsync(new(
                _stringFormater.Format(_applicationResourceService.GetResource<string>("Lang.CreatingNewItem"),
                    _applicationResourceService.GetResource<string>("Lang.Credential")), credential,
                new DialogButton(_applicationResourceService.GetResource<string>("Lang.Create"), CreateCommand,
                    credential, DialogButtonType.Primary), UiHelper.CancelButton));

            await InitializedAsync(cancellationToken);
        });
    }

    [RelayCommand]
    private Task CreateAsync(CredentialParametersViewModel parametersViewModel, CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            parametersViewModel.StartExecute();

            if (parametersViewModel.HasErrors)
            {
                return;
            }

            await _credentialService.AddAsync(new()
            {
                Id = parametersViewModel.Id,
                Name = parametersViewModel.Name,
                Login = parametersViewModel.Login,
                Key = parametersViewModel.Key,
                IsAvailableUpperLatin = parametersViewModel.IsAvailableUpperLatin,
                IsAvailableLowerLatin = parametersViewModel.IsAvailableLowerLatin,
                IsAvailableNumber = parametersViewModel.IsAvailableNumber,
                IsAvailableSpecialSymbols = parametersViewModel.IsAvailableSpecialSymbols,
                CustomAvailableCharacters = parametersViewModel.CustomAvailableCharacters,
                Length = parametersViewModel.Length,
                Regex = parametersViewModel.Regex,
                Type = parametersViewModel.Type,
            }, cancellationToken);

            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
    }
}