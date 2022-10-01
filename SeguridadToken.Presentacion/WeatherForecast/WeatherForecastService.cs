using Newtonsoft.Json;
using SeguridadToken.Dto;
using System.Net.Http.Headers;

namespace SeguridadToken.Presentacion.WeatherForecast;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(HttpClient httpClient, ILogger<WeatherForecastService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<IEnumerable<WeatherForecastDto>> GetWeatherForecast()
    {
        var token = Environment.GetEnvironmentVariable("token");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync("/WeatherForecast");

        var weatherForecast = JsonConvert.DeserializeObject<IEnumerable<WeatherForecastDto>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

        if (weatherForecast == null)
            _logger.LogError("Error" + "IEnumerable<WeatherForecastDto> null");

        return weatherForecast;
    }
}
