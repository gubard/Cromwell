using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Db;
using Cromwell.Helpers;
using Cromwell.Models;
using Cromwell.Services;

namespace Cromwell.Ui;

public partial class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IApplicationResourceService _applicationResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly ICredentialService _credentialService;
    private EditCredentialViewModel? _createCredential;

    public MainViewModel(
        IDialogService dialogService,
        IApplicationResourceService applicationResourceService,
        IStringFormater stringFormater,
        ICredentialService credentialService
    )
    {
        _dialogService = dialogService;
        _applicationResourceService = applicationResourceService;
        _stringFormater = stringFormater;
        _credentialService = credentialService;
        SelectedCredential = Credentials.FirstOrDefault();
    }

    public AvaloniaList<EditCredentialViewModel> Credentials { get; } = new();

    [ObservableProperty]
    public partial EditCredentialViewModel? SelectedCredential { get; set; }

    [RelayCommand]
    private Task ShowCreateViewAsync(CancellationToken cancellationToken)
    {
        _createCredential = new(Guid.CreateVersion7());

        return WrapCommand(async () =>
        {
            await _dialogService.ShowMessageBoxAsync(new(
                _stringFormater.Format(_applicationResourceService.GetResource<string>("Lang.CreatingNewItem"),
                    _applicationResourceService.GetResource<string>("Lang.Credential")), _createCredential,
                new DialogButton(_applicationResourceService.GetResource<string>("Lang.Create"), CreateCommand,
                    DialogButtonType.Primary), UiHelper.CancelButton));

            await InitializedAsync(cancellationToken);
        });
    }

    [RelayCommand]
    private Task CreateCredentialAsync(EditCredentialViewModel viewModel, CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
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

    private EditCredentialViewModel[] Fill(CredentialEntity[] items)
    {
        var result = items.Where(x => x.ParentId == null).Select(x => new EditCredentialViewModel(x)).ToArray();

        foreach (var item in result)
        {
            Fill(item, items);
        }

        return result;
    }

    private void Fill(EditCredentialViewModel viewModel, CredentialEntity[] items)
    {
        var children = items.Where(x => x.ParentId == viewModel.Id)
           .Select(x => new EditCredentialViewModel(x))
           .ToArray();

        viewModel.Children.AddRange(children);

        foreach (var child in children)
        {
            Fill(child, items);
        }
    }

    [RelayCommand]
    private Task DeleteAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            if (SelectedCredential is null)
            {
                return;
            }

            await _credentialService.DeleteAsync(SelectedCredential.Id, cancellationToken);
            await InitializedAsync(cancellationToken);
        });
    }

    [RelayCommand]
    private Task SaveAsync()
    {
        return WrapCommand(() => Task.CompletedTask);
    }

    [RelayCommand]
    private Task CreateAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            if (_createCredential is null)
            {
                return;
            }

            _createCredential.StartExecute();

            if (_createCredential.HasErrors)
            {
                return;
            }

            await _credentialService.AddAsync(new()
            {
                Id = _createCredential.Id,
                Name = _createCredential.Name,
                Login = _createCredential.Login,
                Key = _createCredential.Key,
                IsAvailableUpperLatin = _createCredential.IsAvailableUpperLatin,
                IsAvailableLowerLatin = _createCredential.IsAvailableLowerLatin,
                IsAvailableNumber = _createCredential.IsAvailableNumber,
                IsAvailableSpecialSymbols = _createCredential.IsAvailableSpecialSymbols,
                CustomAvailableCharacters = _createCredential.CustomAvailableCharacters,
                Length = _createCredential.Length,
                Regex = _createCredential.Regex,
                Type = _createCredential.Type,
            }, cancellationToken);

            _dialogService.CloseMessageBox();
            await InitializedAsync(cancellationToken);
        });
    }
}