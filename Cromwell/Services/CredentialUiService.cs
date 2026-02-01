using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface ICredentialUiService
    : IUiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class CredentialUiService(
    ICredentialHttpService credentialHttpService,
    ICredentialDbService dbService,
    AppState appState,
    ICredentialUiCache uiCache,
    INavigator navigator,
    string serviceName,
    IResponseHandler responseHandler
)
    : UiService<
        TurtleGetRequest,
        TurtlePostRequest,
        TurtleGetResponse,
        TurtlePostResponse,
        ICredentialHttpService,
        ICredentialDbService,
        ICredentialUiCache
    >(credentialHttpService, dbService, appState, uiCache, navigator, serviceName, responseHandler),
        ICredentialUiService;
