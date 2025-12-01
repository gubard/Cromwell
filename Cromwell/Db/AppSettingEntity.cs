using Nestor.Db;
using Nestor.Db.Models;

namespace Cromwell.Db;

[SourceEntity(nameof(Id))]
public partial class AppSettingEntity
{
    public Guid Id { get; set; }
    public string GeneralKey { get; set; } = string.Empty;
    public ThemeVariantType Theme { get; set; }
}