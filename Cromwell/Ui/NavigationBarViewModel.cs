using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using IServiceProvider = Inanna.Services.IServiceProvider;

namespace Cromwell.Ui;

public partial class NavigationBarViewModel : ViewModelBase
{
    private readonly INavigator _navigator;
    private readonly IAppSettingService _appSettingService;
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IApplicationResourceService _appResourceService;

    public NavigationBarViewModel(
        INavigator navigator,
        IAppSettingService appSettingService,
        IDialogService dialogService,
        IServiceProvider serviceProvider,
        IApplicationResourceService appResourceService
    )
    {
        _navigator = navigator;
        _appSettingService = appSettingService;
        _dialogService = dialogService;
        _serviceProvider = serviceProvider;
        _appResourceService = appResourceService;

        _navigator.ViewChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsCanBack));
            OnPropertyChanged(nameof(Header));
        };
    }

    public bool IsCanBack => !_navigator.IsEmpty;

    public object Header
    {
        get
        {
            if (_navigator.CurrentView is null)
            {
                return new TextBlock
                {
                    Text = _appResourceService.GetResource<string>("Lang.Cromwell"),
                    Classes = { "alignment-left-center", "h3", },
                };
            }

            if (_navigator.CurrentView is IHeader header)
            {
                return header.Header;
            }

            return new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.Cromwell"),
                Classes = { "alignment-left-center", "h3",},
            };
        }
    }

    [RelayCommand]
    private Task ShowSettingsViewAsync(CancellationToken cancellationToken)
    {
        var setting = _serviceProvider.GetService<AppSettingViewModel>();

        return WrapCommand(() => _dialogService.ShowMessageBoxAsync(new(
            _appResourceService.GetResource<string>("Lang.Settings"),
            _serviceProvider.GetService<AppSettingViewModel>(),
            new DialogButton(_appResourceService.GetResource<string>("Lang.Save"), SaveSettingsCommand, setting,
                DialogButtonType.Primary), UiHelper.CancelButton)));
    }

    [RelayCommand]
    private Task BackAsync(CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            await _navigator.NavigateBackOrNullAsync(cancellationToken);
            OnPropertyChanged(nameof(IsCanBack));
        });
    }

    [RelayCommand]
    private Task SaveSettingsAsync(AppSettingViewModel setting, CancellationToken cancellationToken)
    {
        return WrapCommand(async () =>
        {
            await _appSettingService.SaveAppSettingsAsync(new()
            {
                GeneralKey = setting.GeneralKey,
                Id = Guid.Empty,
                Theme = setting.Theme,
            }, cancellationToken);

            _dialogService.CloseMessageBox();
        });
    }
}