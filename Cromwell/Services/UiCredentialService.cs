using Inanna.Models;
using Turtle.Contract.Models;
using Turtle.Contract.Services;

namespace Cromwell.Services;

public interface IUiCredentialService : ICredentialService;

public class UiCredentialService : IUiCredentialService
{
    private readonly IHttpCredentialService _httpCredentialService;
    private readonly IEfCredentialService _efCredentialService;
    private readonly AppState _appState;
    private long _lastHttpId;

    public UiCredentialService(IHttpCredentialService httpCredentialService, IEfCredentialService efCredentialService, AppState appState)
    {
        _httpCredentialService = httpCredentialService;
        _efCredentialService = efCredentialService;
        _appState = appState;
        _lastHttpId = -1;
    }

    public async ValueTask<TurtleGetResponse> GetAsync(TurtleGetRequest request, CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                await InitAsync(ct);

                return await _httpCredentialService.GetAsync(request, ct);
            }
            case AppMode.Offline:
                return await _efCredentialService.GetAsync(request, ct);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async ValueTask<TurtlePostResponse> PostAsync(TurtlePostRequest request, CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                await InitAsync(ct);
                var lastLocalId = await _efCredentialService.GetLastIdAsync(ct);
                request.LastLocalId = lastLocalId;
                var response = await _httpCredentialService.PostAsync(request, ct);
                await _efCredentialService.SaveEventsAsync(response.Events, ct);

                return response;
            }
            case AppMode.Offline:
                return await _efCredentialService.PostAsync(request, ct);

            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async ValueTask InitAsync(CancellationToken ct)
    {
        if (_lastHttpId != -1)
        {
            return;
        }

        var request = new TurtleGetRequest();
        var lastLocalId = await _efCredentialService.GetLastIdAsync(ct);
        request.LastId = lastLocalId;
        var response = await _httpCredentialService.GetAsync(request, ct);

        if (response.Events.Length == 0)
        {
            return;
        }

        await _efCredentialService.SaveEventsAsync(response.Events, ct);
        _lastHttpId = response.Events.Max(x => x.Id);
    }
}