using Gaia.Services;

namespace Cromwell.Models;

public sealed class CromwellSettings : ObjectStorageValue<CromwellSettings>
{
    public string GeneralKey { get; set; } = DefaultGeneralKey;
    public ThemeVariantType Theme { get; set; }

    private static readonly string DefaultGeneralKey = Guid.NewGuid().ToString().ToUpper();
}
