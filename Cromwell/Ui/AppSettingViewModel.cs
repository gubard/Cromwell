using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Models;

namespace Cromwell.Ui;

public partial class AppSettingViewModel : ViewModelBase
{
    private readonly IAppSettingService  _appSettingService;
    
    public AppSettingViewModel(IAppSettingService appSettingService)
    {
        _appSettingService = appSettingService;
        _generalKey = string.Empty;
    }

    [ObservableProperty] private string _generalKey;
    
    [RelayCommand]
    private Task InitializedAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            var settings = await _appSettingService.GetAppSettingsAsync();
            GeneralKey = settings.GeneralKey;
        });
    }
}