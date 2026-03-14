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

public sealed class CromwellCommands
{
    public CromwellCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        async ValueTask GeneratePasswordAsync(CredentialNotify credential, CancellationToken ct)
        {
            var settings = await _serviceProvider
                .GetService<IObjectStorage>()
                .LoadAsync<CromwellSettings>(ct);
            var key = $"{settings.GeneralKey}{credential.Key}";

            var password = _serviceProvider
                .GetService<IPasswordGeneratorService>()
                .GeneratePassword(
                    key,
                    new(
                        $"{credential.IsAvailableNumber.IfTrueElseEmpty(StringHelper.Number)}{credential.IsAvailableLowerLatin.IfTrueElseEmpty(StringHelper.LowerLatin)}{credential.IsAvailableUpperLatin.IfTrueElseEmpty(StringHelper.UpperLatin)}{credential.IsAvailableSpecialSymbols.IfTrueElseEmpty(StringHelper.SpecialSymbols)}{credential.CustomAvailableCharacters}",
                        credential.Length,
                        credential.Regex
                    )
                );

            await _serviceProvider.GetService<IClipboardService>().SetTextAsync(password, ct);

            _serviceProvider
                .GetService<INotificationService>()
                .ShowNotification(
                    _serviceProvider
                        .GetService<IStringFormater>()
                        .Format(
                            _serviceProvider
                                .GetService<IAppResourceService>()
                                .GetResource<string>("Lang.Copied"),
                            _serviceProvider
                                .GetService<IAppResourceService>()
                                .GetResource<string>("Lang.Password")
                        )
                        .DispatchToNotification(),
                    NotificationType.Success
                );
        }

        _generatePasswordCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (parameters, ct) => GeneratePasswordAsync(parameters, ct).ConfigureAwait(false)
                )
        );

        async ValueTask LoginToClipboardAsync(CredentialNotify parameters, CancellationToken ct)
        {
            await _serviceProvider
                .GetService<IClipboardService>()
                .SetTextAsync(parameters.Login, ct);

            _serviceProvider
                .GetService<INotificationService>()
                .ShowNotification(
                    _serviceProvider
                        .GetService<IStringFormater>()
                        .Format(
                            _serviceProvider
                                .GetService<IAppResourceService>()
                                .GetResource<string>("Lang.Copied"),
                            _serviceProvider
                                .GetService<IAppResourceService>()
                                .GetResource<string>("Lang.Login")
                        )
                        .DispatchToNotification(),
                    NotificationType.Success
                );
        }

        _loginToClipboardCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (parameters, ct) => LoginToClipboardAsync(parameters, ct).ConfigureAwait(false)
                )
        );

        _openCredentialCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (credential, ct) =>
                        _serviceProvider
                            .GetService<INavigator>()
                            .NavigateToAsync(
                                _serviceProvider
                                    .GetService<ICromwellViewModelFactory>()
                                    .CreateCredential(credential),
                                ct
                            )
                )
        );

        _showEditCredentialCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (credential, ct) =>
                    {
                        var parameters = _serviceProvider
                            .GetService<ICromwellViewModelFactory>()
                            .CreateCredentialParameters(credential);

                        return _serviceProvider
                            .GetService<IDialogService>()
                            .ShowMessageBoxAsync(
                                new(
                                    _serviceProvider
                                        .GetService<IStringFormater>()
                                        .Format(
                                            _serviceProvider
                                                .GetService<IAppResourceService>()
                                                .GetResource<string>("Lang.EditItem"),
                                            credential.Name
                                        )
                                        .DispatchToDialogHeader(),
                                    parameters,
                                    _serviceProvider.GetService<ISafeExecuteWrapper>(),
                                    new(
                                        _serviceProvider
                                            .GetService<IAppResourceService>()
                                            .GetResource<string>("Lang.Edit"),
                                        _serviceProvider
                                            .GetService<ICommandFactory>()
                                            .CreateCommand(async c =>
                                            {
                                                await _serviceProvider
                                                    .GetService<IDialogService>()
                                                    .CloseMessageBoxAsync(c);

                                                return await _serviceProvider
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
                                    _serviceProvider.GetService<IDialogService>().CancelButton
                                ),
                                ct
                            );
                    }
                )
        );

        _showDeleteCredentialCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (credential, ct) =>
                        _serviceProvider
                            .GetService<IDialogService>()
                            .ShowMessageBoxAsync(
                                new(
                                    _serviceProvider
                                        .GetService<IAppResourceService>()
                                        .GetResource<string>("Lang.Delete")
                                        .DispatchToDialogHeader(),
                                    _serviceProvider
                                        .GetService<IStringFormater>()
                                        .Format(
                                            _serviceProvider
                                                .GetService<IAppResourceService>()
                                                .GetResource<string>("Lang.DeleteAsk"),
                                            credential.Name
                                        ),
                                    _serviceProvider.GetService<ISafeExecuteWrapper>(),
                                    new(
                                        _serviceProvider
                                            .GetService<IAppResourceService>()
                                            .GetResource<string>("Lang.Delete"),
                                        _serviceProvider
                                            .GetService<ICommandFactory>()
                                            .CreateCommand(async c =>
                                            {
                                                await _serviceProvider
                                                    .GetService<IDialogService>()
                                                    .CloseMessageBoxAsync(c);

                                                return await _serviceProvider
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
                                    _serviceProvider.GetService<IDialogService>().CancelButton
                                ),
                                ct
                            )
                )
        );

        _showChangeParentCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (item, ct) =>
                    {
                        var viewModel = _serviceProvider
                            .GetService<ICromwellViewModelFactory>()
                            .CreateChangeParentCredential();

                        Dispatcher.UIThread.Post(() =>
                        {
                            _serviceProvider.GetService<ICredentialUiCache>().ResetItems();
                            item.IsHideOnTree = true;
                        });

                        return _serviceProvider
                            .GetService<IDialogService>()
                            .ShowMessageBoxAsync(
                                new(
                                    _serviceProvider
                                        .GetService<IStringFormater>()
                                        .Format(
                                            _serviceProvider
                                                .GetService<IAppResourceService>()
                                                .GetResource<string>("Lang.ChangeParentItem"),
                                            item.Name
                                        )
                                        .DispatchToDialogHeader(),
                                    viewModel,
                                    _serviceProvider.GetService<ISafeExecuteWrapper>(),
                                    new(
                                        _serviceProvider
                                            .GetService<IAppResourceService>()
                                            .GetResource<string>("Lang.ChangeParent"),
                                        _serviceProvider
                                            .GetService<ICommandFactory>()
                                            .CreateCommand(async c =>
                                            {
                                                var parentId = viewModel.IsRoot
                                                    ? null
                                                    : viewModel.Tree.Selected?.Id;

                                                await _serviceProvider
                                                    .GetService<IDialogService>()
                                                    .CloseMessageBoxAsync(c);

                                                return await _serviceProvider
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
                                    _serviceProvider.GetService<IDialogService>().CancelButton
                                ),
                                ct
                            );
                    }
                )
        );

        _openLinkCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<CredentialNotify>(
                    (item, ct) =>
                        _serviceProvider
                            .GetService<IOpenerLink>()
                            .OpenLinkAsync(item.Link.ToUri(), ct)
                )
        );
    }

    public ICommand OpenLinkCommand => _openLinkCommand.Value;
    public ICommand GeneratePasswordCommand => _generatePasswordCommand.Value;
    public ICommand LoginToClipboardCommand => _loginToClipboardCommand.Value;
    public ICommand OpenCredentialCommand => _openCredentialCommand.Value;
    public ICommand ShowEditCredentialCommand => _showEditCredentialCommand.Value;
    public ICommand ShowDeleteCredentialCommand => _showDeleteCredentialCommand.Value;
    public ICommand ShowChangeParentCommand => _showChangeParentCommand.Value;

    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<ICommand> _openLinkCommand;
    private readonly Lazy<ICommand> _generatePasswordCommand;
    private readonly Lazy<ICommand> _loginToClipboardCommand;
    private readonly Lazy<ICommand> _openCredentialCommand;
    private readonly Lazy<ICommand> _showEditCredentialCommand;
    private readonly Lazy<ICommand> _showDeleteCredentialCommand;
    private readonly Lazy<ICommand> _showChangeParentCommand;
}
