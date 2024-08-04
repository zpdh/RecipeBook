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

    private void ChangeCulture(string culture)
    {
        if (_httpClient.DefaultRequestHeaders.Contains("Accept-Language"))
        {
            _httpClient.DefaultRequestHeaders.Remove("Accept-Language");
        }

        _httpClient.DefaultRequestHeaders.Add("Accept-Language", culture);
    }
}