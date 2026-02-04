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
    >(credentialHttpService, dbService, uiCache, navigator, serviceName, responseHandler),
        ICredentialUiService;
