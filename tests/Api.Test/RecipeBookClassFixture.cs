using System.Collections;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;

namespace Api.Test;

public class RecipeBookClassFixture : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _httpClient;

    protected RecipeBookClassFixture(CustomWebApplicationFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    protected async Task<HttpResponseMessage> Post<T>(
        string endpoint,
        T request,
        string token = "",
        string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.PostAsJsonAsync(endpoint, request);
    }

    protected async Task<HttpResponseMessage> PostAsFormData(
        string endpoint,
        object request,
        string token = "",
        string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);

        var formData = new MultipartFormDataContent();

        var requestProperties = request.GetType().GetProperties().ToList();

        // Form data works similarly to a dictionary,
        // therefore it's needed to get all properties
        // and add them in a (key = value) pair.
        foreach (var property in requestProperties)
        {
            var propertyValue = property.GetValue(request);

            if (string.IsNullOrWhiteSpace(propertyValue?.ToString())) continue;

            if (propertyValue is IList list)
            {
                AddListToFormData(formData, property.Name, list);
                continue;
            }

            formData.Add(
                new StringContent(propertyValue.ToString()!),
                property.Name);
        }

        return await _httpClient.PostAsync(endpoint, formData);
    }

    protected async Task<HttpResponseMessage> Get(
        string endpoint,
        string token = "",
        string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.GetAsync(endpoint);
    }

    protected async Task<HttpResponseMessage> Put<T>(
        string endpoint,
        T request,
        string token = "",
        string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.PutAsJsonAsync(endpoint, request);
    }

    protected async Task<HttpResponseMessage> Delete(
        string endpoint,
        string token = "",
        string culture = "en")
    {
        ChangeCulture(culture);
        AuthorizeRequest(token);

        return await _httpClient.DeleteAsync(endpoint);
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

    private static void AddListToFormData(
        MultipartFormDataContent content,
        string propertyName,
        IList list)
    {
        var itemType = list.GetType().GetGenericArguments().Single();

        if (itemType.IsClass && itemType != typeof(string)) // String is a class so this check is necessary
        {
            AddClassListToFormData(content, propertyName, list);
            return;
        }

        foreach (var item in list)
        {
            content.Add(
                new StringContent(item.ToString()!),
                propertyName);
        }
    }

    private static void AddClassListToFormData(
        MultipartFormDataContent content,
        string propertyName,
        IList list)
    {
        var index = 0;

        foreach (var item in list)
        {
            var classProperties = item.GetType().GetProperties().ToList();

            foreach (var property in classProperties)
            {
                var value = property.GetValue(item, null);

                content.Add(
                    new StringContent(value!.ToString()!),
                    // Format: objProperty[positionInList][property]
                    $"{propertyName}[{index}][{property.Name}]");
            }

            index++;
        }
    }
}