using MVCPrototype.Domain.Entities;

namespace MVCPrototype.Application.Services
{
    public interface IWeatherService
    {
        try
        {
            Task<List<WeatherForecast>> GetWeatherAPI();
            Task<WeatherForecast> GetTemperatureByAPI(string date,string apiKey, bool isMax);
        }
        catch (Exception ex)
        {
            IEnumerable<WeatherForecast> GetWeather();
            Console.WriteLine("Ocorreu um erro: " + ex.Message);
        }
    }
}
