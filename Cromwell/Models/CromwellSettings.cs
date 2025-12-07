using Nestor.Db.Models;

namespace Cromwell.Models;

[SourceEntity(nameof(Id))]
public partial class CromwellSettings
{
    private static readonly string DefaultGeneralKey = Guid.Empty.ToString().ToUpper();
    
    public Guid Id { get; set; }
    public string GeneralKey { get; set; } = DefaultGeneralKey;
    public ThemeVariantType Theme { get; set; }
}