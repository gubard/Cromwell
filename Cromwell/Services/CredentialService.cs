using System.Text.Json;
using Gaia.Services;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public class CredentialService(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions, ITryPolicyService tryPolicyService) : Service<TurtleGetRequest, TurtlePostRequest, TurtleGetResponse, TurtlePostResponse>(httpClient, jsonSerializerOptions, tryPolicyService), ICredentialService;