using System.Windows.Input;
using Avalonia.Controls;
using Cromwell.Models;
using Cromwell.Services;
using Cromwell.Ui;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;

namespace Cromwell.Helpers;

public static class CromwellCommands
{
    static CromwellCommands()
    {
        var dialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        var uiCredentialService = DiHelper.ServiceProvider.GetService<IUiCredentialService>();
        var objectStorage = DiHelper.ServiceProvider.GetService<IObjectStorage>();
        var appResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        var passwordGeneratorService =
            DiHelper.ServiceProvider.GetService<IPasswordGeneratorService>();
        var clipboardService = DiHelper.ServiceProvider.GetService<IClipboardService>();
        var notificationService = DiHelper.ServiceProvider.GetService<INotificationService>();
        var stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();

        async ValueTask GeneratePasswordAsync(CredentialNotify parameters, CancellationToken ct)
        {
            var settings = await objectStorage.LoadAsync<CromwellSettings>(
                $"{typeof(CromwellSettings).FullName}",
                ct
            );

            var generalKey = settings?.GeneralKey ?? Guid.NewGuid().ToString();

            var password = passwordGeneratorService.GeneratePassword(
                $"{generalKey}{parameters.Key}",
                new(
                    $"{parameters.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{parameters.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{parameters.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{parameters.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{parameters.CustomAvailableCharacters}",
                    parameters.Length,
                    parameters.Regex
                )
            );

            await clipboardService.SetTextAsync(password, ct);

            notificationService.ShowNotification(
                new TextBlock
                {
                    Text = stringFormater.Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService.GetResource<string>("Lang.Password")
                    ),
                    Classes = { "align-center", "m-5", "h2" },
                },
                NotificationType.Success
            );
        }

        GeneratePasswordCommand = UiHelper.CreateCommand<CredentialNotify>(
            (parameters, ct) => GeneratePasswordAsync(parameters, ct).ConfigureAwait(false)
        );

        async ValueTask LoginToClipboardAsync(CredentialNotify parameters, CancellationToken ct)
        {
            await clipboardService.SetTextAsync(parameters.Login, ct);

            notificationService.ShowNotification(
                new TextBlock
                {
                    Text = stringFormater.Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService.GetResource<string>("Lang.Login")
                    ),
                    Classes = { "align-center", "m-5", "h2" },
                },
                NotificationType.Success
            );
        }

        LoginToClipboardCommand = UiHelper.CreateCommand<CredentialNotify>(
            (parameters, ct) => LoginToClipboardAsync(parameters, ct).ConfigureAwait(false)
        );

        OpenCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (parameters, ct) =>
                navigator.NavigateToAsync(
                    new CredentialViewModel(
                        uiCredentialService,
                        dialogService,
                        stringFormater,
                        appResourceService,
                        parameters
                    ),
                    ct
                )
        );

        EditCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (credential, ct) =>
                navigator.NavigateToAsync(
                    new EditCredentialViewModel(
                        credential,
                        uiCredentialService,
                        notificationService,
                        appResourceService,
                        stringFormater
                    ),
                    ct
                )
        );

        DeleteCredentialCommand = UiHelper.CreateCommand<CredentialNotify, TurtlePostResponse>(
            (credential, ct) =>
                uiCredentialService.PostAsync(
                    Guid.NewGuid(),
                    new() { DeleteIds = [credential.Id] },
                    ct
                )
        );
    }

    public static readonly ICommand GeneratePasswordCommand;
    public static readonly ICommand LoginToClipboardCommand;
    public static readonly ICommand OpenCredentialCommand;
    public static readonly ICommand EditCredentialCommand;
    public static readonly ICommand DeleteCredentialCommand;
}
