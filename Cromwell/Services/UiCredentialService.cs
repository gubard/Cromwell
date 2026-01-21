using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface IUiCredentialService
    : IUiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class UiCredentialService(
    IHttpCredentialService httpService,
    IEfCredentialService efService,
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
        IHttpCredentialService,
        IEfCredentialService,
        ICredentialUiCache
    >(httpService, efService, appState, uiCache, navigator, serviceName, responseHandler),
        IUiCredentialService;
