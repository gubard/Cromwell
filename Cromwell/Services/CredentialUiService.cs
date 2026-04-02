using Gaia.Services;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface ICredentialUiService
    : IUiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class CredentialUiService(
    ICredentialHttpService credentialHttpService,
    ICredentialDbService dbService,
    ICredentialUiCache uiCache,
    INavigator navigator,
    string serviceName,
    IStatusBarService statusBarService,
    IInannaViewModelFactory factory
)
    : UiService<
        TurtleGetRequest,
        TurtlePostRequest,
        TurtleGetResponse,
        TurtlePostResponse,
        ICredentialHttpService,
        ICredentialDbService,
        ICredentialUiCache
    >(credentialHttpService, dbService, uiCache, navigator, serviceName, statusBarService, factory),
        ICredentialUiService
{
    protected override async ValueTask<IValidationErrors> RefreshServiceCore(CancellationToken ct)
    {
        var request = new TurtleGetRequest
        {
            IsGetBookmarks = true,
            IsGetRoots = true,
            IsGetSelectors = true,
        };

        var response = await DbService.GetAsync(request, ct);
        await UiCache.UpdateAsync(response, ct);

        return response;
    }
}
