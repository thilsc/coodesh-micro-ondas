using System.Text.Json;
using System.Text;

namespace MicroondasCliente.Models;

public static class ApiResponseHelper
{
    private static IHttpClientFactory? _httpClientFactory;
    private static readonly string _ApiName = "MicroondasApi";

    public static void Initialize(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private static HttpClient GetClient()
    {
        if (_httpClientFactory == null)
            throw new InvalidOperationException("ApiResponseHelper não foi inicializado. Chame Initialize() no Program.cs");
        
        return _httpClientFactory.CreateClient(_ApiName);
    }

    #region GET Requests
    public static async Task<HttpResponseMessage> ObterRetornoEndpointApiGet(string endpoint)
    {
        var client = GetClient();
        return await client.GetAsync(endpoint);
    }

    public static async Task<T?> ObterRetornoEndpointApiGet<T>(string endpoint) where T : class
    {
        var response = await ObterRetornoEndpointApiGet(endpoint);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }
    #endregion

    #region POST Requests (sem payload)
    public static async Task<HttpResponseMessage> ObterRetornoEndpointApiPost(string endpoint)
    {
        var client = GetClient();
        return await client.PostAsync(endpoint, null);
    }

    public static async Task<T?> ObterRetornoEndpointApiPost<T>(string endpoint) where T : class
    {
        var response = await ObterRetornoEndpointApiPost(endpoint);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }
    #endregion

    #region POST Requests (com payload)
    public static async Task<HttpResponseMessage> ObterRetornoEndpointApiPost(string endpoint, object? payload)
    {
        if (payload == null)
            return await ObterRetornoEndpointApiPost(endpoint);

        var client = GetClient();
        var jsonContent = new StringContent(
            JsonSerializer.Serialize(payload), 
            Encoding.UTF8, 
            "application/json"
        );
        return await client.PostAsync(endpoint, jsonContent);
    }

    public static async Task<T?> ObterRetornoEndpointApiPost<T>(string endpoint, object? payload) where T : class
    {
        var response = await ObterRetornoEndpointApiPost(endpoint, payload);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }
    #endregion

    #region DELETE Requests
    public static async Task<HttpResponseMessage> ObterRetornoEndpointApiDelete(string endpoint)
    {
        var client = GetClient();
        return await client.DeleteAsync(endpoint);
    }

    public static async Task<T?> ObterRetornoEndpointApiDelete<T>(string endpoint) where T : class
    {
        var response = await ObterRetornoEndpointApiDelete(endpoint);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<T>(json, options);
        }

        return null;
    }
    #endregion
}