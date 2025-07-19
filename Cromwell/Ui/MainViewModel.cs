using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Helpers;
using Cromwell.Models;
using Cromwell.Services;

namespace Cromwell.Ui;

public partial class MainViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly IApplicationResourceService _applicationResourceService;
    private readonly IStringFormater _stringFormater;
    private readonly IViewModelFactory _viewModelFactory;
    private readonly ICredentialService _credentialService;

    public MainViewModel(
        IDialogService dialogService,
        IApplicationResourceService applicationResourceService,
        IStringFormater stringFormater,
        IViewModelFactory viewModelFactory,
        ICredentialService credentialService
    )
    {
        _dialogService = dialogService;
        _applicationResourceService = applicationResourceService;
        _stringFormater = stringFormater;
        _viewModelFactory = viewModelFactory;
        _credentialService = credentialService;
        SelectedCredential = Credentials.FirstOrDefault();
    }

    public AvaloniaList<EditCredentialViewModel> Credentials { get; } = new();

    [ObservableProperty]
    public partial EditCredentialViewModel? SelectedCredential { get; set; }

    [RelayCommand]
    private Task ShowCreateViewAsync(CancellationToken cancellationToken)
    {
        var viewModel = _viewModelFactory.CreateEditCredentialViewModel(Guid.CreateVersion7());

        return WrapCommand(async () =>
        {
            await _dialogService.ShowMessageBoxAsync(new(
                _stringFormater.Format(_applicationResourceService.GetResource<string>("Lang.CreatingNewItem"),
                    _applicationResourceService.GetResource<string>("Lang.Credential")), viewModel,
                new DialogButton(_applicationResourceService.GetResource<string>("Lang.Create"),
                    viewModel.CreateCommand, DialogButtonType.Primary), UiHelper.CancelButton));

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
            Credentials.AddRange(credentials.Select(x => _viewModelFactory.CreateEditCredentialViewModel(x)));
        });
    }
}