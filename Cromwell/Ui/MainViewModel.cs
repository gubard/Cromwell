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

    public MainViewModel(
        IDialogService dialogService,
        IApplicationResourceService applicationResourceService,
        IStringFormater stringFormater
    )
    {
        _dialogService = dialogService;
        _applicationResourceService = applicationResourceService;
        _stringFormater = stringFormater;
        SelectedCredential = Credentials.FirstOrDefault();
    }

    public AvaloniaList<EditCredentialViewModel> Credentials { get; } = new();

    [ObservableProperty]
    public partial EditCredentialViewModel? SelectedCredential { get; set; }

    [RelayCommand]
    private Task ShowCreateViewAsync()
    {
        var viewModel = new EditCredentialViewModel(Guid.CreateVersion7());

        return WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _stringFormater.Format(_applicationResourceService.GetResource<string>("Lang.CreatingNewItem"),
                _applicationResourceService.GetResource<string>("Lang.Credential")), viewModel,
            new DialogButton(_applicationResourceService.GetResource<string>("Lang.Create"), viewModel.CreateCommand,
                DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private Task CreateCredentialAsync(EditCredentialViewModel viewModel)
    {
        return WrapCommand(async () =>
        {
            _dialogService.CloseMessageBox();
            await InitializedAsync();
        });
    }

    [RelayCommand]
    private Task InitializedAsync()
    {
        return WrapCommand(() => Task.CompletedTask);
    }
}