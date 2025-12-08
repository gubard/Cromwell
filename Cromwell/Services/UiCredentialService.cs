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
    AppState appState)
    :
        UiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse,
            TurtlePostResponse, HttpCredentialService, EfCredentialService>(
            service, efService, appState), IUiCredentialService;