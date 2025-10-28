using Nestor.Db;

namespace Cromwell.Db;

[SourceEntity(nameof(Id))]
public partial class AppSettingEntity
{
    public Guid Id { get; set; }
    public string GeneralKey { get; set; } = string.Empty;
}