using System.Text.Json;
using Gaia.Models;
using Gaia.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public class HttpCredentialService(
    HttpClient httpClient,
    JsonSerializerOptions jsonSerializerOptions,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory)
    : HttpService<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse,
        TurtlePostResponse>(httpClient, jsonSerializerOptions, tryPolicyService,
        headersFactory), IHttpCredentialService;