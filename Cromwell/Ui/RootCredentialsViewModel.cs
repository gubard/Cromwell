using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Services;

namespace Cromwell.Ui;

public partial class RootCredentialsViewModel : ViewModelBase
{
    private readonly ICredentialService _credentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IApplicationResourceService _appResourceService;
    private readonly INavigator _navigator;
    private readonly INotificationService _notificationService;

    public RootCredentialsViewModel(
        ICredentialService credentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IApplicationResourceService appResourceService,
        INavigator navigator,
        INotificationService notificationService
    )
    {
        _credentialService = credentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _appResourceService = appResourceService;
        _navigator = navigator;
        _notificationService = notificationService;
    }

    public AvaloniaList<CredentialParametersViewModel> Credentials { get; } = new();

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            Credentials.Clear();
            var response = await _credentialService.GetAsync(new()
            {
                IsGetRoots = true,
            }, cancellationToken);

            Credentials.AddRange(response.Roots.OrderBy(x => x.OrderIndex)
               .Select(x => new CredentialParametersViewModel(x)));
        });
    }

    [RelayCommand]
    private async Task EditAsync(CredentialParametersViewModel credential, CancellationToken cancellationToken)
    {
        await WrapCommand(() =>
            _navigator.NavigateToAsync(
                new EditCredentialViewModel(credential, _credentialService, _notificationService,
                    _appResourceService), cancellationToken));
    }

    [RelayCommand]
    private async Task DeleteAsync(CredentialParametersViewModel parametersViewModel, CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            await _credentialService.PostAsync(new()
            {
                DeleteIds = [parametersViewModel.Id],
            }, cancellationToken);
            await InitializedAsync(cancellationToken);
        });
    }

    [RelayCommand]
    private async Task ShowCreateViewAsync(CancellationToken cancellationToken)
    {
        var credential = new CredentialParametersViewModel(Guid.CreateVersion7());

        await WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _stringFormater.Format(_appResourceService.GetResource<string>("Lang.CreatingNewItem"),
                _appResourceService.GetResource<string>("Lang.Credential")), credential,
            new DialogButton(_appResourceService.GetResource<string>("Lang.Create"), CreateCommand,
                credential, DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private async Task CreateAsync(CredentialParametersViewModel parametersViewModel, CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            parametersViewModel.StartExecute();

            if (parametersViewModel.HasErrors)
            {
                return;
            }

            await _credentialService.PostAsync(new()
            {
                CreateCredentials =
                [
                    new()
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
                    },
                ],
            }, cancellationToken);

            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
    }
}