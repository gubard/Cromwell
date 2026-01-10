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
    ICredentialCache cache,
    INavigator navigator,
    string serviceName
)
    : UiService<
        TurtleGetRequest,
        TurtlePostRequest,
        TurtleGetResponse,
        TurtlePostResponse,
        IHttpCredentialService,
        IEfCredentialService,
        ICredentialCache
    >(httpService, efService, appState, cache, navigator, serviceName),
        IUiCredentialService;
