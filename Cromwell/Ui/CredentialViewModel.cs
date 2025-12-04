using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Services;

namespace Cromwell.Ui;

public partial class CredentialViewModel : ViewModelBase, IHeader
{
    [ObservableProperty] private IEnumerable _parents;

    private readonly ICredentialService _credentialService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IApplicationResourceService _applicationResourceService;
    private readonly INavigator _navigator;
    private readonly INotificationService _notificationService;

    public CredentialViewModel(
        ICredentialService credentialService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IApplicationResourceService applicationResourceService,
        INavigator navigator,
        INotificationService notificationService,
        CredentialParametersViewModel credential
    )
    {
        _credentialService = credentialService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _applicationResourceService = applicationResourceService;
        _navigator = navigator;
        _notificationService = notificationService;
        Credential = credential;
        Header = new CredentialHeaderViewModel(credential);
        _parents = Array.Empty<object>();
    }

    public CredentialParametersViewModel Credential { get; }
    public object Header { get; }

    [RelayCommand]
    private Task InitializedAsync(CancellationToken ct)
    {
        return WrapCommand(async () =>
        {
            Credential.Children.Clear();
            var response = await _credentialService.GetAsync(new()
            {
                GetChildrenIds = [Credential.Id],
                GetParentsIds = [Credential.Id],
            }, ct);

            Credential.Children.AddRange(response.Children[Credential.Id].OrderBy(x => x.OrderIndex)
               .Select(x => new CredentialParametersViewModel(x)));

            Parents = Root.Instance
               .Cast<object>()
               .ToEnumerable()
               .Concat(response.Parents[Credential.Id].Select(x => new CredentialParametersViewModel(x)))
               .ToArray();
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
            await _credentialService.PostAsync(new()
            {
                DeleteIds = [parametersViewModel.Id],
            }, cancellationToken);
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
                            ParentId = Credential.Id,
                        },
                    ],
                }
                , cancellationToken);

            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
    }
}