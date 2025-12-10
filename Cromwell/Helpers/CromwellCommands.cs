using System.Windows.Input;
using Avalonia.Controls;
using Cromwell.Models;
using Cromwell.Services;
using Cromwell.Ui;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;

namespace Cromwell.Helpers;

public static class CromwellCommands
{
    static CromwellCommands()
    {
        var dialogService =
            DiHelper.ServiceProvider.GetService<IDialogService>();
        var uiCredentialService =
            DiHelper.ServiceProvider.GetService<IUiCredentialService>();
        var appSettingService = DiHelper.ServiceProvider
           .GetService<ISettingsService<CromwellSettings>>();
        var appResourceService =
            DiHelper.ServiceProvider.GetService<IAppResourceService>();
        var passwordGeneratorService = DiHelper.ServiceProvider
           .GetService<IPasswordGeneratorService>();
        var clipboardService =
            DiHelper.ServiceProvider.GetService<IClipboardService>();
        var notificationService =
            DiHelper.ServiceProvider.GetService<INotificationService>();
        var stringFormater =
            DiHelper.ServiceProvider.GetService<IStringFormater>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();

        GeneratePasswordCommand = UiHelper.CreateCommand<CredentialNotify>(
            async (parameters, ct) =>
            {
                var settings = await appSettingService.GetSettingsAsync(ct);

                var password = passwordGeneratorService.GeneratePassword(
                    $"{settings.GeneralKey}{parameters.Key}",
                    new(
                        $"{parameters.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{parameters.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{parameters.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{parameters.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{parameters.CustomAvailableCharacters}",
                        parameters.Length, parameters.Regex));

                await clipboardService.SetTextAsync(password, ct);

                notificationService.ShowNotification(new TextBlock
                {
                    Text = stringFormater.Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService
                           .GetResource<string>("Lang.Password")),
                    Classes =
                    {
                        "alignment-center",
                    },
                }, NotificationType.Success);
            });

        LoginToClipboardCommand = UiHelper.CreateCommand<CredentialNotify>(
            async (parameters, ct) =>
            {
                await clipboardService.SetTextAsync(parameters.Login, ct);

                notificationService.ShowNotification(new TextBlock
                {
                    Text = stringFormater.Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService.GetResource<string>("Lang.Login")),
                    Classes =
                    {
                        "alignment-center",
                    },
                }, NotificationType.Success);
            });

        OpenCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (parameters, ct) =>
                navigator.NavigateToAsync(
                    new CredentialViewModel(uiCredentialService, dialogService,
                        stringFormater, appResourceService, parameters), ct));

        EditCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (credential, ct) =>
                navigator.NavigateToAsync(
                    new EditCredentialViewModel(credential, uiCredentialService,
                        notificationService,
                        appResourceService), ct));

        DeleteCredentialCommand =
            UiHelper.CreateCommand<CredentialNotify, TurtlePostResponse>(
                (credential, ct) =>
                    uiCredentialService.PostAsync(new()
                    {
                        DeleteIds = [credential.Id],
                    }, ct));
    }

    public static readonly ICommand GeneratePasswordCommand;
    public static readonly ICommand LoginToClipboardCommand;
    public static readonly ICommand OpenCredentialCommand;
    public static readonly ICommand EditCredentialCommand;
    public static readonly ICommand DeleteCredentialCommand;
}