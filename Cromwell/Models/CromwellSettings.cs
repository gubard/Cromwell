namespace Cromwell.Models;

public sealed class CromwellSettings
{
    private static readonly string DefaultGeneralKey = Guid.NewGuid().ToString().ToUpper();

    public string GeneralKey { get; set; } = DefaultGeneralKey;
    public ThemeVariantType Theme { get; set; }
}
