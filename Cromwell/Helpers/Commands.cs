using System.Windows.Input;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Cromwell.Services;
using Cromwell.Ui;
using Gaia.Extensions;
using Gaia.Helpers;
using Inanna.Enums;
using Inanna.Helpers;
using Inanna.Services;

namespace Cromwell.Helpers;

public static class Commands
{
    static Commands()
    {
        var dialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        var credentialService = DiHelper.ServiceProvider.GetService<ICredentialService>();
        var appSettingService = DiHelper.ServiceProvider.GetService<IAppSettingService>();
        var appResourceService = DiHelper.ServiceProvider.GetService<IApplicationResourceService>();
        var passwordGeneratorService = DiHelper.ServiceProvider.GetService<IPasswordGeneratorService>();
        var clipboardService = DiHelper.ServiceProvider.GetService<IClipboardService>();
        var notificationService = DiHelper.ServiceProvider.GetService<INotificationService>();
        var stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        var mainViewModel = DiHelper.ServiceProvider.GetService<MainViewModel>();

        GeneratePasswordCommand = new AsyncRelayCommand<CredentialParametersViewModel>(async (parameters, ct) =>
        {
            var settings = await appSettingService.GetAppSettingsAsync();

            var password = passwordGeneratorService.GeneratePassword($"{settings.GeneralKey}{parameters.Key}",
                new(
                    $"{parameters.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{parameters.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{parameters.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{parameters.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{parameters.CustomAvailableCharacters}",
                    parameters.Length, parameters.Regex));

            await clipboardService.SetTextAsync(password, ct);

            notificationService.ShowNotification(new TextBlock
            {
                Text = stringFormater.Format(appResourceService.GetResource<string>("Lang.Copied"),
                    appResourceService.GetResource<string>("Lang.Password")),
                Classes =
                {
                    "alignment-center",
                },
            }, NotificationType.Success);
        });

        LoginToClipboardCommand = new AsyncRelayCommand<CredentialParametersViewModel>(async (parameters, ct) =>
        {
            await clipboardService.SetTextAsync(parameters.Login, ct);

            notificationService.ShowNotification(new TextBlock
            {
                Text = stringFormater.Format(appResourceService.GetResource<string>("Lang.Copied"),
                    appResourceService.GetResource<string>("Lang.Login")),
                Classes =
                {
                    "alignment-center",
                },
            }, NotificationType.Success);
        });

        OpenCredentialCommand = new AsyncRelayCommand<CredentialParametersViewModel>((parameters, ct) =>
            navigator.NavigateToAsync(
                new CredentialViewModel(credentialService, dialogService, stringFormater, appResourceService, navigator,
                    notificationService, parameters), ct));

        NavigateToRootCredentialsViewCommand = new AsyncRelayCommand(ct =>
            navigator.NavigateToAsync(
                new RootCredentialsViewModel(credentialService, dialogService, stringFormater, appResourceService,
                    navigator, notificationService), ct));

        ShowPaneCommand = new AsyncRelayCommand(ct =>
        {
            mainViewModel.IsShowPane = true;

            return Task.CompletedTask;
        });

        HidePaneCommand = new AsyncRelayCommand(ct =>
        {
            mainViewModel.IsShowPane = false;

            return Task.CompletedTask;
        });
    }

    public static readonly ICommand GeneratePasswordCommand;
    public static readonly ICommand LoginToClipboardCommand;
    public static readonly ICommand OpenCredentialCommand;
    public static readonly ICommand NavigateToRootCredentialsViewCommand;
    public static readonly ICommand ShowPaneCommand;
    public static readonly ICommand HidePaneCommand;
}