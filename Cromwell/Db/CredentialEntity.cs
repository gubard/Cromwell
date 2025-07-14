using CommunityToolkit.Mvvm.ComponentModel;
using Nestor.Db;

namespace Cromwell.Db;

[SourceEntity(nameof(Id))]
public partial class CredentialEntity : ObservableObject
{
    [ObservableProperty]
    public partial Guid Id { get; set; }

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
    public partial string CustomAvailableCharacters { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ushort Length { get; set; }

    [ObservableProperty]
    public partial string Regex { get; set; } = string.Empty;

    [ObservableProperty]
    public partial CredentialType Type { get; set; }

    [ObservableProperty]
    public partial uint OrderIndex { get; set; }

    [ObservableProperty]
    public partial Guid? ParentId { get; set; }
}