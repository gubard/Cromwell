using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Db;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Extensions;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

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
    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            Credential.Children.Clear();
            var parents = await _credentialService.GetParentsAsync(Credential.Id, cancellationToken);
            var credentials = await _credentialService.GetChildrenAsync(Credential.Id, cancellationToken);

            Credential.Children.AddRange(credentials.OrderBy(x => x.OrderIndex)
               .Select(x => new CredentialParametersViewModel(x)));

            Parents = Root.Instance
               .Cast<object>()
               .ToEnumerable()
               .Concat(parents.Select(x => new CredentialParametersViewModel(x)))
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
                ParentId = Credential.Id,
            }, cancellationToken);

            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
    }

    private CredentialParametersViewModel[] Fill(CredentialEntity[] items)
    {
        var result = items.Where(x => x.ParentId == null)
           .OrderBy(x => x.OrderIndex)
           .Select(x => new CredentialParametersViewModel(x))
           .ToArray();

        foreach (var item in result)
        {
            Fill(item, items);
        }

        return result;
    }

    private void Fill(CredentialParametersViewModel parametersViewModel, CredentialEntity[] items)
    {
        var children = items.Where(x => x.ParentId == parametersViewModel.Id)
           .OrderBy(x => x.OrderIndex)
           .Select(x => new CredentialParametersViewModel(x))
           .ToArray();

        parametersViewModel.Children.AddRange(children);

        foreach (var child in children)
        {
            Fill(child, items);
        }
    }
}