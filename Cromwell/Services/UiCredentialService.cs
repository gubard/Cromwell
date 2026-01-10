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
    ICredentialMemoryCache memoryCache,
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
        ICredentialMemoryCache
    >(httpService, efService, appState, memoryCache, navigator, serviceName),
        IUiCredentialService;
