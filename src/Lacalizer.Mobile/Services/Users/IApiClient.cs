
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Lacalizer.Mobile.Services.Users;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken ct = default);
    Task PutAsync<TRequest>(string url, TRequest request, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private IConfiguration _config;

    public ApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;

        //_client.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);

    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var response = await _httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }

    public async Task PutAsync<TRequest>(string url, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PutAsJsonAsync(url, request, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync(url, ct);
        response.EnsureSuccessStatusCode();
    }
}