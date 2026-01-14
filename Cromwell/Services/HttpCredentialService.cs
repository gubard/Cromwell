using System.Text.Json;
using Gaia.Models;
using Gaia.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public sealed class HttpCredentialService(
    HttpClient httpClient,
    JsonSerializerOptions options,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>(
        httpClient,
        options,
        tryPolicyService,
        headersFactory
    ),
        IHttpCredentialService
{
    protected override TurtleGetRequest CreateHealthCheckGetRequest()
    {
        return new();
    }
}
