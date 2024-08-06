using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;

namespace Api.Test;

public class RecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;
    
    protected RecipeBookClassFixture(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> Post<T>(string endpoint, T request, string culture = "en")
    {
        ChangeCulture(culture);
        
        return await _httpClient.PostAsJsonAsync(endpoint, request);
    }
    
    protected async Task<HttpResponseMessage> Get(string endpoint, string token = "", string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);
        
        return await _httpClient.GetAsync(endpoint);
    }

    private void ChangeCulture(string culture)
    {
        if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
        {
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
        }

        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
    }

    private void AuthorizeRequest(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return;

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}