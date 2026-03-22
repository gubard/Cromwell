using System.ComponentModel;
using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Services;
using Gaia.Helpers;
using Gaia.Services;
using IconPacks.Avalonia.MaterialDesign;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Cromwell.Models;

public sealed partial class CredentialNotify
    : ObservableObject,
        IStaticServiceFactory<Guid, CredentialNotify>,
        IIsDrag
{
    public Guid Id { get; }
    public IAvaloniaReadOnlyList<CredentialNotify> Children => _children;
    public IEnumerable<object> Parents => _parents;
    public IAvaloniaReadOnlyList<InannaCommand> Commands => _commands;

    [ObservableProperty]
    public partial bool IsDrag { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Key { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Link { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsAvailableUpperLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableLowerLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableNumber { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableSpecialSymbols { get; set; }

    [ObservableProperty]
    public partial string CustomAvailableCharacters { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ushort Length { get; set; }

    [ObservableProperty]
    public partial string Regex { get; set; } = string.Empty;

    [ObservableProperty]
    public partial CredentialType Type { get; set; }

    [ObservableProperty]
    public partial bool IsBookmark { get; set; }

    [ObservableProperty]
    public partial CredentialNotify? Parent { get; set; }

    [ObservableProperty]
    public partial uint OrderIndex { get; set; }

    public static CredentialNotify Create(Guid input, IServiceProvider serviceProvider)
    {
        return new(
            input,
            serviceProvider.GetService<IAppResourceService>(),
            serviceProvider.GetService<CromwellCommands>()
        );
    }

    public void UpdateParents(IEnumerable<CredentialNotify> parents)
    {
        var allParents = HomeMark.Instance.Cast<object>().ToEnumerable().Concat(parents).ToArray();
        _parents.UpdateOrder(allParents);
    }

    public void UpdateChildren(CredentialNotify[] children)
    {
        _children.UpdateOrder(children);
    }

    public void AddChild(CredentialNotify child)
    {
        _children.AddSorted(child, x => x.OrderIndex);
    }

    public void RemoveChild(CredentialNotify child)
    {
        _children.Remove(child);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Type))
        {
            _commands.Clear();

            if (Type == CredentialType.Value)
            {
                _commands.AddRange([
                    new(
                        _cromwellCommands.ShowDeleteCredentialCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Delete"),
                        PackIconMaterialDesignKind.Delete,
                        ButtonType.Danger
                    ),
                    new(
                        _cromwellCommands.ShowEditCredentialCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Edit"),
                        PackIconMaterialDesignKind.Edit
                    ),
                    new(
                        _cromwellCommands.ShowChangeParentCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.ChangeParent"),
                        PackIconMaterialDesignKind.AccountTree
                    ),
                    new(
                        _cromwellCommands.OpenLinkCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.OpenLink"),
                        PackIconMaterialDesignKind.Link
                    ),
                    new(
                        _cromwellCommands.LoginToClipboardCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Login"),
                        PackIconMaterialDesignKind.Login
                    ),
                    new(
                        _cromwellCommands.GeneratePasswordCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Password"),
                        PackIconMaterialDesignKind.Password
                    ),
                ]);
            }
            else
            {
                _commands.AddRange([
                    new(
                        _cromwellCommands.ShowDeleteCredentialCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Delete"),
                        PackIconMaterialDesignKind.Delete,
                        ButtonType.Danger
                    ),
                    new(
                        _cromwellCommands.ShowEditCredentialCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.Edit"),
                        PackIconMaterialDesignKind.Edit
                    ),
                    new(
                        _cromwellCommands.ShowChangeParentCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.ChangeParent"),
                        PackIconMaterialDesignKind.AccountTree
                    ),
                    new(
                        _cromwellCommands.OpenLinkCommand,
                        this,
                        _appResourceService.GetResource<string>("Lang.OpenLink"),
                        PackIconMaterialDesignKind.Link
                    ),
                ]);
            }
        }
    }

    [ObservableProperty]
    private bool _isHideOnTree;

    private readonly AvaloniaList<object> _parents = [];
    private readonly AvaloniaList<InannaCommand> _commands;
    private readonly IAppResourceService _appResourceService;
    private readonly AvaloniaList<CredentialNotify> _children = [];
    private readonly CromwellCommands _cromwellCommands;

    private CredentialNotify(
        Guid id,
        IAppResourceService appResourceService,
        CromwellCommands cromwellCommands
    )
    {
        Id = id;
        _appResourceService = appResourceService;
        _cromwellCommands = cromwellCommands;

        _commands =
        [
            new(
                _cromwellCommands.ShowDeleteCredentialCommand,
                this,
                appResourceService.GetResource<string>("Lang.Delete"),
                PackIconMaterialDesignKind.Delete
            ),
            new(
                _cromwellCommands.ShowEditCredentialCommand,
                this,
                appResourceService.GetResource<string>("Lang.Edit"),
                PackIconMaterialDesignKind.Edit
            ),
            new(
                _cromwellCommands.ShowChangeParentCommand,
                this,
                appResourceService.GetResource<string>("Lang.ChangeParent"),
                PackIconMaterialDesignKind.AccountTree
            ),
            new(
                _cromwellCommands.OpenLinkCommand,
                this,
                appResourceService.GetResource<string>("Lang.OpenLink"),
                PackIconMaterialDesignKind.Link
            ),
            new(
                _cromwellCommands.LoginToClipboardCommand,
                this,
                appResourceService.GetResource<string>("Lang.Login"),
                PackIconMaterialDesignKind.Login
            ),
            new(
                _cromwellCommands.GeneratePasswordCommand,
                this,
                appResourceService.GetResource<string>("Lang.Password"),
                PackIconMaterialDesignKind.Password
            ),
        ];
    }
}
