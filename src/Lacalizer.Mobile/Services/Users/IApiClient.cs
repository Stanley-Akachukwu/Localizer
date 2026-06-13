
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
        if (_httpClient == null)
            throw new Exception("_httpClient is NULL");

        if (request == null)
            throw new Exception("request is NULL");
        try
        {
            Console.WriteLine($"BaseUrl JSON: {_httpClient.BaseAddress}");
            var json = JsonSerializer.Serialize(request);
            Console.WriteLine($"REQUEST JSON: {json}");

            var response = await _httpClient.PostAsync(
                url,
                new StringContent(json, Encoding.UTF8, "application/json"),
                ct);

            var content = await response.Content.ReadAsStringAsync(ct);

            Console.WriteLine($"BODY: {content}");
            Console.WriteLine($"Status Code: {response.StatusCode}");
            Console.WriteLine($"Reason: {response.ReasonPhrase}");
            Console.WriteLine($"Final URI: {response.RequestMessage?.RequestUri}");

            return JsonSerializer.Deserialize<TResponse>(
                content,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"POST FAILED: {ex}");
            throw;
        }
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