namespace SampleDotNetOTEL.ProxyService.ExternalServices;

public sealed class BusinessServiceClient
{
    private readonly HttpClient _httpClient;

    public BusinessServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<string> GetWeatherAsync()
    {
        var response = await _httpClient.GetAsync("weather");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    public async Task<string> GetHelloAsync()
    {
        var response = await _httpClient.GetAsync("hello");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    public async Task<string> GetHelloAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync("hello", new { Name = name });
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}