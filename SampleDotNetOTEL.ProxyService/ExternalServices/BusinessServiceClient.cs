namespace SampleDotNetOTEL.ProxyService.ExternalServices;

public sealed class BusinessServiceClient(HttpClient httpClient)
{
    public async Task<string> GetWeatherAsync()
    {
        var response = await httpClient.GetAsync("weather");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    public async Task<string> GetHelloAsync()
    {
        var response = await httpClient.GetAsync("hello");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
    
    public async Task<string> GetHelloAsync(string name)
    {
        var response = await httpClient.PostAsJsonAsync("hello", new { Name = name });
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}