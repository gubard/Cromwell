using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Services;
using Turtle.Contract.Models;

namespace Cromwell.Models;

public partial class CredentialNotify : ObservableObject,
    IStaticFactory<Guid, CredentialNotify>
{
    private CredentialNotify(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
    public AvaloniaList<CredentialNotify> Children { get; } = new();
    public AvaloniaList<CredentialNotify> Parents { get; } = new();

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

    public static CredentialNotify Create(Guid input)
    {
        return new(input);
    }
}