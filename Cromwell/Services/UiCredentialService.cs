using Inanna.Models;
using Inanna.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface IUiCredentialService : IUiService<TurtleGetRequest,
    TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>;

public sealed class UiCredentialService(
    CredentialHttpService httpService,
    EfCredentialService efService,
    AppState appState)
    :
        UiService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse,
            TurtlePostResponse, CredentialHttpService, EfCredentialService>(
            httpService, efService, appState), IUiCredentialService;