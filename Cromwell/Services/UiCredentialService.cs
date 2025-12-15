using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface IUiCredentialService
    : IUiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class UiCredentialService(
    IHttpCredentialService service,
    IEfCredentialService efService,
    AppState appState,
    ICredentialCache cache,
    INavigator navigator
)
    : UiService<
        TurtleGetRequest,
        TurtlePostRequest,
        TurtleGetResponse,
        TurtlePostResponse,
        IHttpCredentialService,
        IEfCredentialService,
        ICredentialCache
    >(service, efService, appState, cache, navigator),
        IUiCredentialService;
