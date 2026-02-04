using Cromwell.Services;
using Inanna.Controls;
using Turtle.Contract.Models;

namespace Cromwell.Controls;

public abstract class CredentialDropUserControl
    : DropUserControl<
        ICredentialUiService,
        TurtleGetRequest,
        TurtlePostRequest,
        TurtleGetResponse,
        TurtlePostResponse,
        EditCredential
    >;
