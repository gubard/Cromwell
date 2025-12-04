using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Models;

namespace Cromwell.Ui;

public partial class AppSettingViewModel : ViewModelBase
{
    [ObservableProperty] private string _generalKey;
    [ObservableProperty] private ThemeVariantType _theme;

    private readonly IAppSettingService _appSettingService;
    private readonly Application _application;

    public AppSettingViewModel(IAppSettingService appSettingService, Application application)
    {
        _appSettingService = appSettingService;
        _application = application;
        _generalKey = string.Empty;
    }

    [RelayCommand]
    private async Task InitializedAsync(CancellationToken cancellationToken)
    {
        await WrapCommand(async () =>
        {
            var settings = await _appSettingService.GetAppSettingsAsync();
            GeneralKey = settings.GeneralKey;
            Theme = settings.Theme;
        });
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Theme))
        {
            _application.RequestedThemeVariant = Theme switch
            {
                ThemeVariantType.Default => ThemeVariant.Default,
                ThemeVariantType.Dark => ThemeVariant.Dark,
                ThemeVariantType.Light => ThemeVariant.Light,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }
}