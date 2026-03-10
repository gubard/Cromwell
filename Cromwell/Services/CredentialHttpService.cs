using System.Text.Json;
using Gaia.Models;
using Gaia.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public sealed class CredentialHttpService(
    IFactory<HttpClient> httpClientFactory,
    JsonSerializerOptions options,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>(
        httpClientFactory,
        options,
        tryPolicyService,
        headersFactory
    ),
        ICredentialHttpService
{
    protected override TurtleGetRequest CreateHealthCheckGetRequest()
    {
        return new();
    }
}
