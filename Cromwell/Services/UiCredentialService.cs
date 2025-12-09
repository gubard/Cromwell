using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface IUiCredentialService : IUiService<TurtleGetRequest,
    TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class UiCredentialService(
    HttpCredentialService service,
    EfCredentialService efService,
    AppState appState,
    ICredentialCache cache)
    :
        UiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse,
            TurtlePostResponse, HttpCredentialService, EfCredentialService,
            ICache<TurtleGetResponse>>(
            service, efService, appState, cache), IUiCredentialService;