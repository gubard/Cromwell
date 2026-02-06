using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Cromwell.Models;
using Cromwell.Services;
using Inanna.Models;
using Inanna.Services;

namespace Cromwell.Ui;

public sealed partial class CredentialTreeViewModel : ViewModelBase, IInitUi
{
    public CredentialTreeViewModel(
        ICredentialUiCache credentialUiCache,
        ICredentialUiService credentialUiService
    )
    {
        _credentialUiService = credentialUiService;
        Roots = credentialUiCache.Roots;
        _selected = Roots.FirstOrDefault();
    }

    public IEnumerable<CredentialNotify> Roots { get; }

    public ConfiguredValueTaskAwaitable InitUiAsync(CancellationToken ct)
    {
        return WrapCommandAsync(
            () => _credentialUiService.GetAsync(new() { IsGetSelectors = true }, ct),
            ct
        );
    }

    private readonly ICredentialUiService _credentialUiService;

    [ObservableProperty]
    private CredentialNotify? _selected;
}
