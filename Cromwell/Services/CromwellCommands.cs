using System.Windows.Input;
using Avalonia.Threading;
using Cromwell.Models;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Services;

public sealed class CromwellCommands : Commands
{
    public CromwellCommands(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _generatePasswordCommand = CreateLazyCommand<CredentialNotify>(
            async (credential, ct) =>
            {
                var settings = await ServiceProvider
                    .GetService<IObjectStorage>()
                    .LoadAsync<CromwellSettings>(ct);
                var key = $"{settings.GeneralKey}{credential.Key}";

                var password = ServiceProvider
                    .GetService<IPasswordGeneratorService>()
                    .GeneratePassword(
                        key,
                        new(
                            $"{credential.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{credential.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{credential.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{credential.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{credential.CustomAvailableCharacters}",
                            credential.Length,
                            credential.Regex
                        )
                    );

                await ServiceProvider.GetService<IClipboardService>().SetTextAsync(password, ct);

                ServiceProvider
                    .GetService<INotificationService>()
                    .ShowNotification(
                        ServiceProvider
                            .GetService<IStringFormater>()
                            .Format(
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Copied"),
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Password")
                            )
                            .DispatchToNotification(),
                        NotificationType.Success
                    );
            }
        );

        _loginToClipboardCommand = CreateLazyCommand<CredentialNotify>(
            async (parameters, ct) =>
            {
                await ServiceProvider
                    .GetService<IClipboardService>()
                    .SetTextAsync(parameters.Login, ct);

                ServiceProvider
                    .GetService<INotificationService>()
                    .ShowNotification(
                        ServiceProvider
                            .GetService<IStringFormater>()
                            .Format(
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Copied"),
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Login")
                            )
                            .DispatchToNotification(),
                        NotificationType.Success
                    );
            }
        );

        _openCredentialCommand = CreateLazyCommand<CredentialNotify>(
            (credential, ct) =>
                ServiceProvider
                    .GetService<INavigator>()
                    .NavigateToAsync(
                        ServiceProvider
                            .GetService<ICromwellViewModelFactory>()
                            .CreateCredential(credential),
                        ct
                    )
        );

        _showEditCredentialCommand = CreateLazyCommand<CredentialNotify>(
            (credential, ct) =>
            {
                var parameters = ServiceProvider
                    .GetService<ICromwellViewModelFactory>()
                    .CreateCredentialParameters(credential);

                return ServiceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        new(
                            ServiceProvider
                                .GetService<IStringFormater>()
                                .Format(
                                    ServiceProvider
                                        .GetService<IAppResourceService>()
                                        .GetResource<string>("Lang.EditItem"),
                                    credential.Name
                                )
                                .DispatchToDialogHeader(),
                            parameters,
                            ServiceProvider.GetService<ISafeExecuteWrapper>(),
                            new(
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Edit"),
                                ServiceProvider
                                    .GetService<ICommandFactory>()
                                    .CreateCommand(async c =>
                                    {
                                        await ServiceProvider
                                            .GetService<IDialogService>()
                                            .CloseMessageBoxAsync(c);

                                        return await ServiceProvider
                                            .GetService<ICredentialUiService>()
                                            .PostAsync(
                                                Guid.NewGuid(),
                                                new()
                                                {
                                                    Edits =
                                                    [
                                                        parameters.CreateEditCredential(
                                                            credential.Id
                                                        ),
                                                    ],
                                                },
                                                c
                                            );
                                    }),
                                null,
                                DialogButtonType.Primary
                            ),
                            ServiceProvider.GetService<IDialogService>().CancelButton
                        ),
                        ct
                    );
            }
        );

        _showDeleteCredentialCommand = CreateLazyCommand<CredentialNotify>(
            (credential, ct) =>
                ServiceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        new(
                            ServiceProvider
                                .GetService<IAppResourceService>()
                                .GetResource<string>("Lang.Delete")
                                .DispatchToDialogHeader(),
                            ServiceProvider
                                .GetService<IStringFormater>()
                                .Format(
                                    ServiceProvider
                                        .GetService<IAppResourceService>()
                                        .GetResource<string>("Lang.DeleteAsk"),
                                    credential.Name
                                ),
                            ServiceProvider.GetService<ISafeExecuteWrapper>(),
                            new(
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.Delete"),
                                ServiceProvider
                                    .GetService<ICommandFactory>()
                                    .CreateCommand(async c =>
                                    {
                                        await ServiceProvider
                                            .GetService<IDialogService>()
                                            .CloseMessageBoxAsync(c);

                                        return await ServiceProvider
                                            .GetService<ICredentialUiService>()
                                            .PostAsync(
                                                Guid.NewGuid(),
                                                new() { DeleteIds = [credential.Id] },
                                                ct
                                            );
                                    }),
                                null,
                                DialogButtonType.Primary
                            ),
                            ServiceProvider.GetService<IDialogService>().CancelButton
                        ),
                        ct
                    )
        );

        _showChangeParentCommand = CreateLazyCommand<CredentialNotify>(
            (item, ct) =>
            {
                var viewModel = ServiceProvider
                    .GetService<ICromwellViewModelFactory>()
                    .CreateChangeParentCredential();

                Dispatcher.UIThread.Post(() =>
                {
                    ServiceProvider.GetService<ICredentialUiCache>().ResetItems();
                    item.IsHideOnTree = true;
                });

                return ServiceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        new(
                            ServiceProvider
                                .GetService<IStringFormater>()
                                .Format(
                                    ServiceProvider
                                        .GetService<IAppResourceService>()
                                        .GetResource<string>("Lang.ChangeParentItem"),
                                    item.Name
                                )
                                .DispatchToDialogHeader(),
                            viewModel,
                            ServiceProvider.GetService<ISafeExecuteWrapper>(),
                            new(
                                ServiceProvider
                                    .GetService<IAppResourceService>()
                                    .GetResource<string>("Lang.ChangeParent"),
                                ServiceProvider
                                    .GetService<ICommandFactory>()
                                    .CreateCommand(async c =>
                                    {
                                        var parentId = viewModel.IsRoot
                                            ? null
                                            : viewModel.Tree.Selected?.Id;

                                        await ServiceProvider
                                            .GetService<IDialogService>()
                                            .CloseMessageBoxAsync(c);

                                        return await ServiceProvider
                                            .GetService<ICredentialUiService>()
                                            .PostAsync(
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
                            ServiceProvider.GetService<IDialogService>().CancelButton
                        ),
                        ct
                    );
            }
        );

        _openLinkCommand = CreateLazyCommand<CredentialNotify>(
            (item, ct) =>
                ServiceProvider.GetService<IOpenerLink>().OpenLinkAsync(item.Link.ToUri(), ct)
        );
    }

    public ICommand OpenLinkCommand => _openLinkCommand.Value;
    public ICommand GeneratePasswordCommand => _generatePasswordCommand.Value;
    public ICommand LoginToClipboardCommand => _loginToClipboardCommand.Value;
    public ICommand OpenCredentialCommand => _openCredentialCommand.Value;
    public ICommand ShowEditCredentialCommand => _showEditCredentialCommand.Value;
    public ICommand ShowDeleteCredentialCommand => _showDeleteCredentialCommand.Value;
    public ICommand ShowChangeParentCommand => _showChangeParentCommand.Value;

    private readonly Lazy<ICommand> _openLinkCommand;
    private readonly Lazy<ICommand> _generatePasswordCommand;
    private readonly Lazy<ICommand> _loginToClipboardCommand;
    private readonly Lazy<ICommand> _openCredentialCommand;
    private readonly Lazy<ICommand> _showEditCredentialCommand;
    private readonly Lazy<ICommand> _showDeleteCredentialCommand;
    private readonly Lazy<ICommand> _showChangeParentCommand;
}
