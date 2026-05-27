using Lacalizer.Mobile.Services.Users;

namespace Lacalizer.Mobile.Helpers;

public class JwtHandler : DelegatingHandler
{
    private readonly SessionService _sessionService;

    public JwtHandler(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    protected override async Task<HttpResponseMessage>
        SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
    {
        var token = await _sessionService
            .GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new System.Net.Http.Headers
                .AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}