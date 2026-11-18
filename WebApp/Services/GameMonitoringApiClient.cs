using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace WebApp.Services;


/// Service class used by the MVC application to call the WebAPI through HttpClient instead of accessing the database directly.

public class GameMonitoringApiClient : IGameMonitoringApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    
    /// Initializes the API client, stores HttpClient/context services, and sets the API base URL from configuration.
   
    public GameMonitoringApiClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5111/api/");
    }

    
    /// Sends a GET request to the API, adds the JWT token when available, and converts the JSON response into the requested type.
   
    public async Task<T?> GetAsync<T>(string url)
    {
        AddBearerToken();
        return await _httpClient.GetFromJsonAsync<T>(url);
    }

    
    /// Sends a POST request to the API with a request body and returns only success or error information.
   
    public async Task<(bool Success, string? Error)> PostAsync<TRequest>(string url, TRequest body)
    {
        AddBearerToken();
        var response = await _httpClient.PostAsJsonAsync(url, body);
        return await ToResult(response);
    }

    
    /// Sends a POST request to the API and converts a successful JSON response into the requested response type.
   
    public async Task<(bool Success, TResponse? Data, string? Error)> PostAsync<TRequest, TResponse>(string url, TRequest body)
    {
        AddBearerToken();
        var response = await _httpClient.PostAsJsonAsync(url, body);
        if (!response.IsSuccessStatusCode)
            return (false, default, await response.Content.ReadAsStringAsync());

        var data = await response.Content.ReadFromJsonAsync<TResponse>();
        return (true, data, null);
    }

    
    /// Sends a PUT request to the API with updated data and returns success or error information.
   
    public async Task<(bool Success, string? Error)> PutAsync<TRequest>(string url, TRequest body)
    {
        AddBearerToken();
        var response = await _httpClient.PutAsJsonAsync(url, body);
        return await ToResult(response);
    }

    
    /// Sends a DELETE request to the API and returns success or error information.
   
    public async Task<(bool Success, string? Error)> DeleteAsync(string url)
    {
        AddBearerToken();
        var response = await _httpClient.DeleteAsync(url);
        return await ToResult(response);
    }

    
    /// Reads the JWT token from the current session and adds it to the Authorization header for API calls.
   
    private void AddBearerToken()
    {
        var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
        _httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    
    /// Converts an HTTP response into the simple success/error tuple used by MVC controllers.
   
    private static async Task<(bool Success, string? Error)> ToResult(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return (true, null);

        return (false, await response.Content.ReadAsStringAsync());
    }
}
