namespace WebApp.Services;


/// Interface that defines the HTTP helper methods used by MVC controllers to communicate with the WebAPI.

public interface IGameMonitoringApiClient
{
    
    /// Sends a GET request to the API and converts the JSON response into the requested type.
   
    Task<T?> GetAsync<T>(string url);

    
    /// Sends a POST request to the API and returns success or error information.
   
    Task<(bool Success, string? Error)> PostAsync<TRequest>(string url, TRequest body);

    
    /// Sends a POST request to the API and returns both success/error information and response data.
   
    Task<(bool Success, TResponse? Data, string? Error)> PostAsync<TRequest, TResponse>(string url, TRequest body);

    
    /// Sends a PUT request to the API and returns success or error information.
   
    Task<(bool Success, string? Error)> PutAsync<TRequest>(string url, TRequest body);

    
    /// Sends a DELETE request to the API and returns success or error information.
   
    Task<(bool Success, string? Error)> DeleteAsync(string url);
}
