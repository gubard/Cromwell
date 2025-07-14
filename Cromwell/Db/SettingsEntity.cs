using Nestor.Db;

namespace Cromwell.Db;

[SourceEntity(nameof(Id))]
public partial class SettingsEntity
{
    public static Guid MainId = Guid.Parse("A670E472-9B31-4177-B2A1-6AD1F4CEFC90");

    public Guid Id { get; set; } = MainId;
}