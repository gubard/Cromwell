using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Services;
using Inanna.Helpers;
using Turtle.Contract.Models;

namespace Cromwell.Models;

public partial class CredentialNotify : ObservableObject,
    IStaticFactory<Guid, CredentialNotify>
{
    private readonly AvaloniaList<object> _parents = [];

    private CredentialNotify(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
    public AvaloniaList<CredentialNotify> Children { get; } = new();
    public IEnumerable<object> Parents => _parents;

    [ObservableProperty]
    public partial bool IsDrag { get; set; }

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Key { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsAvailableUpperLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableLowerLatin { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableNumber { get; set; }

    [ObservableProperty]
    public partial bool IsAvailableSpecialSymbols { get; set; }

    [ObservableProperty]
    public partial string CustomAvailableCharacters { get; set; } =
        string.Empty;

    [ObservableProperty]
    public partial ushort Length { get; set; }

    [ObservableProperty]
    public partial string Regex { get; set; } = string.Empty;

    [ObservableProperty]
    public partial CredentialType Type { get; set; }

    [ObservableProperty]
    public partial CredentialNotify? Parent { get; set; }

    public static CredentialNotify Create(Guid input)
    {
        return new(input);
    }

    public void UpdateParents(IEnumerable<CredentialNotify> parents)
    {
        _parents.UpdateOrder(Root.IEnumerableInstance
           .Concat(parents.OfType<object>()).ToArray());
    }
}