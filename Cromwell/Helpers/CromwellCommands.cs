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
        var uiCredentialService = DiHelper.ServiceProvider.GetService<ICredentialUiService>();
        var objectStorage = DiHelper.ServiceProvider.GetService<IObjectStorage>();
        var appResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        var clipboardService = DiHelper.ServiceProvider.GetService<IClipboardService>();
        var notificationService = DiHelper.ServiceProvider.GetService<INotificationService>();
        var stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();

        var passwordGeneratorService =
            DiHelper.ServiceProvider.GetService<IPasswordGeneratorService>();

        async ValueTask GeneratePasswordAsync(CredentialNotify credential, CancellationToken ct)
        {
            var settings = await objectStorage.LoadAsync<CromwellSettings>(ct);
            var key = $"{settings.GeneralKey}{credential.Key}";

            var password = passwordGeneratorService.GeneratePassword(
                key,
                new(
                    $"{credential.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{credential.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{credential.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{credential.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{credential.CustomAvailableCharacters}",
                    credential.Length,
                    credential.Regex
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
