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

public partial class CredentialsTreeViewModel : ViewModelBase
{
    private readonly ICredentialService _credentialService;
    private readonly IClipboardService _clipboardService;
    private readonly IAppSettingService _appSettingService;
    private readonly IPasswordGeneratorService _passwordGeneratorService;
    private readonly IDialogService _dialogService;
    private readonly IStringFormater _stringFormater;
    private readonly IApplicationResourceService _applicationResourceService;
    private readonly INavigator _navigator;

    public CredentialsTreeViewModel(
        ICredentialService credentialService,
        IClipboardService clipboardService,
        IAppSettingService appSettingService,
        IPasswordGeneratorService passwordGeneratorService,
        IDialogService dialogService,
        IStringFormater stringFormater,
        IApplicationResourceService applicationResourceService,
        INavigator navigator
    )
    {
        _credentialService = credentialService;
        _clipboardService = clipboardService;
        _appSettingService = appSettingService;
        _passwordGeneratorService = passwordGeneratorService;
        _dialogService = dialogService;
        _stringFormater = stringFormater;
        _applicationResourceService = applicationResourceService;
        _navigator = navigator;
    }

    public AvaloniaList<CredentialParametersViewModel> Credentials { get; } = new();

    [RelayCommand]
    private Task LoginToClipboardAsync(
        CredentialParametersViewModel parametersViewModel,
        CancellationToken cancellationToken
    )
    {
        return WrapCommand(() => _clipboardService.SetTextAsync(parametersViewModel.Login, cancellationToken));
    }

    [RelayCommand]
    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            Credentials.Clear();
            var credentials = await _credentialService.GetAsync(cancellationToken);
            Credentials.AddRange(Fill(credentials));
        });
    }

    [RelayCommand]
    private Task EditAsync(CredentialParametersViewModel credential, CancellationToken cancellationToken)
    {
        return WrapCommand(() =>
            _navigator.NavigateToAsync(new EditCredentialViewModel(credential, _credentialService), cancellationToken));
    }

    [RelayCommand]
    private Task GeneratePasswordAsync(
        CredentialParametersViewModel parametersViewModel,
        CancellationToken cancellationToken
    )
    {
        return WrapCommand(async () =>
        {
            var settings = await _appSettingService.GetAppSettingsAsync();

            var password = _passwordGeneratorService.GeneratePassword($"{settings.GeneralKey}{parametersViewModel.Key}",
                new(
                    $"{parametersViewModel.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{parametersViewModel.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{parametersViewModel.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{parametersViewModel.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{parametersViewModel.CustomAvailableCharacters}",
                    parametersViewModel.Length, parametersViewModel.Regex));

            await _clipboardService.SetTextAsync(password, cancellationToken);
        });
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