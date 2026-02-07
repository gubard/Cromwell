using System.Windows.Input;
using Avalonia.Threading;
using Cromwell.Models;
using Cromwell.Services;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Helpers;

public static class CromwellCommands
{
    static CromwellCommands()
    {
        var openerLink = DiHelper.ServiceProvider.GetService<IOpenerLink>();
        var credentialUiCache = DiHelper.ServiceProvider.GetService<ICredentialUiCache>();
        var credentialUiService = DiHelper.ServiceProvider.GetService<ICredentialUiService>();
        var objectStorage = DiHelper.ServiceProvider.GetService<IObjectStorage>();
        var appResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        var clipboardService = DiHelper.ServiceProvider.GetService<IClipboardService>();
        var notificationService = DiHelper.ServiceProvider.GetService<INotificationService>();
        var stringFormater = DiHelper.ServiceProvider.GetService<IStringFormater>();
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        var factory = DiHelper.ServiceProvider.GetService<ICromwellViewModelFactory>();
        var dialogService = DiHelper.ServiceProvider.GetService<IDialogService>();

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
                stringFormater
                    .Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService.GetResource<string>("Lang.Password")
                    )
                    .DispatchToNotification(),
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
                stringFormater
                    .Format(
                        appResourceService.GetResource<string>("Lang.Copied"),
                        appResourceService.GetResource<string>("Lang.Login")
                    )
                    .DispatchToNotification(),
                NotificationType.Success
            );
        }

        LoginToClipboardCommand = UiHelper.CreateCommand<CredentialNotify>(
            (parameters, ct) => LoginToClipboardAsync(parameters, ct).ConfigureAwait(false)
        );

        OpenCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (credential, ct) => navigator.NavigateToAsync(factory.CreateCredential(credential), ct)
        );

        ShowEditCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (credential, ct) =>
            {
                var parameters = factory.CreateCredentialParameters(credential);

                return dialogService.ShowMessageBoxAsync(
                    new(
                        stringFormater
                            .Format(
                                appResourceService.GetResource<string>("Lang.EditItem"),
                                credential.Name
                            )
                            .DispatchToDialogHeader(),
                        parameters,
                        new(
                            appResourceService.GetResource<string>("Lang.Edit"),
                            UiHelper.CreateCommand(async c =>
                            {
                                await dialogService.CloseMessageBoxAsync(c);

                                return await credentialUiService.PostAsync(
                                    Guid.NewGuid(),
                                    new()
                                    {
                                        Edits = [parameters.CreateEditCredential(credential.Id)],
                                    },
                                    c
                                );
                            }),
                            null,
                            DialogButtonType.Primary
                        ),
                        UiHelper.CancelButton
                    ),
                    ct
                );
            }
        );

        ShowDeleteCredentialCommand = UiHelper.CreateCommand<CredentialNotify>(
            (credential, ct) =>
                dialogService.ShowMessageBoxAsync(
                    new(
                        appResourceService
                            .GetResource<string>("Lang.Delete")
                            .DispatchToDialogHeader(),
                        stringFormater.Format(
                            appResourceService.GetResource<string>("Lang.DeleteAsk"),
                            credential.Name
                        ),
                        new(
                            appResourceService.GetResource<string>("Lang.Delete"),
                            UiHelper.CreateCommand(async c =>
                            {
                                await dialogService.CloseMessageBoxAsync(c);

                                return await credentialUiService.PostAsync(
                                    Guid.NewGuid(),
                                    new() { DeleteIds = [credential.Id] },
                                    ct
                                );
                            }),
                            null,
                            DialogButtonType.Primary
                        ),
                        UiHelper.CancelButton
                    ),
                    ct
                )
        );

        ShowChangeParentCommand = UiHelper.CreateCommand<CredentialNotify>(
            (item, ct) =>
            {
                var viewModel = factory.ChangeParentCredential();

                Dispatcher.UIThread.Post(() =>
                {
                    credentialUiCache.ResetItems();
                    item.IsHideOnTree = true;
                });

                return dialogService.ShowMessageBoxAsync(
                    new(
                        stringFormater
                            .Format(
                                appResourceService.GetResource<string>("Lang.ChangeParentItem"),
                                item.Name
                            )
                            .DispatchToDialogHeader(),
                        viewModel,
                        new(
                            appResourceService.GetResource<string>("Lang.ChangeParent"),
                            UiHelper.CreateCommand(async c =>
                            {
                                var parentId = viewModel.IsRoot
                                    ? null
                                    : viewModel.Tree.Selected?.Id;

                                await dialogService.CloseMessageBoxAsync(c);

                                return await credentialUiService.PostAsync(
                                    Guid.NewGuid(),
                                    new()
                                    {
                                        Edits =
                                        [
                                            new()
                                            {
                                                Ids = [item.Id],
                                                ParentId = parentId,
                                                IsEditParentId = true,
                                            },
                                        ],
                                    },
                                    c
                                );
                            }),
                            null,
                            DialogButtonType.Primary
                        ),
                        UiHelper.CancelButton
                    ),
                    ct
                );
            }
        );

        OpenLinkCommand = UiHelper.CreateCommand<CredentialNotify>(
            (item, ct) => openerLink.OpenLinkAsync(item.Link.ToUri(), ct)
        );
    }

    public static readonly ICommand OpenLinkCommand;
    public static readonly ICommand GeneratePasswordCommand;
    public static readonly ICommand LoginToClipboardCommand;
    public static readonly ICommand OpenCredentialCommand;
    public static readonly ICommand ShowEditCredentialCommand;
    public static readonly ICommand ShowDeleteCredentialCommand;
    public static readonly ICommand ShowChangeParentCommand;
}
