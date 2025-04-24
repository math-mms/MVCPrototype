using MVCPrototype.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

namespace MVCPrototype.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Raining", "Smog", "Sunny"
        };

        public WeatherService() { }

        public IEnumerable<WeatherForecast> GetWeather()
        {
            return Enumerable.Range(1, 7).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
           .ToArray();
        }

        public async Task<List<WeatherForecast>> GetWeatherAPI()
        {
            var weatherForecasts = new List<WeatherForecast>();
            DateTime currentDate = DateTime.Now;
            // Como não é possivel verificar o futuro com a API Climatempo, então fiz com que ela checasse 10 dias no passado ( o maximo que consegue checar tambem)
            int currentDateMaximum = 7;
            string token = await GetAuthTokenAsync();
            string apiKey = token;
            for (int i = 1; i <= currentDateMaximum; i++)
            {
                DateTime targetDate = currentDate.AddDays(-i);
                string formattedDate = targetDate.ToString("yyyy-MM-dd");

                // Chame a API para pegar a previsão para cada dia
                var forecast = await GetTemperatureByAPI(formattedDate,apiKey, true);

                if (forecast != null)
                    weatherForecasts.Add((WeatherForecast)forecast);
            }

            return weatherForecasts;
        }

        public async Task<WeatherForecast> GetTemperatureByAPI(string date,string apiKey, bool isMax)
        {
            string temperatureMin = "tmin2m";
            string temperatureMax = "tmax2m";

            string longitude = "-45.8872";
            string latitude = "-23.1791";

            string url = "https://api.cnptia.embrapa.br/climapi/v1/ncep-gfs/";
            if (isMax)
            {
                url += $@"{temperatureMax}";
            }
            else
            {
                url += $@"{temperatureMin}";
            }
            url += $@"/{date}/%20{longitude}/%20{latitude}";

            // Criando o cliente HTTP
            using (HttpClient client = new HttpClient())
            {
                // Adicionando o cabeçalho de autenticação (caso seja necessário)
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                try
                {
                    // Enviando a requisição GET
                    HttpResponseMessage response = await client.GetAsync(url);

                    // Verificando se a resposta foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        var listaTempoJson = await response.Content.ReadAsStringAsync();
                        var listaTempo = JsonConvert.DeserializeObject<List<HourlyTemperature>>(listaTempoJson);
                        if (listaTempo != null && listaTempo.Any())
                        {
                            // Encontrando a temperatura máxima (ou mínima, dependendo do parâmetro `isMax`)
                            double maxTemperature = listaTempo.Max(t => t.valor); // Encontrando o valor máximo da temperatura

                            // Criando o objeto WeatherForecast
                            var forecast = new WeatherForecast
                            {
                                Date = DateOnly.Parse(date),
                                TemperatureC = (int)maxTemperature,
                                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                            };

                            return forecast;
                        }
                        return null;
                    }
                    else
                    {
                        throw new Exception($@"Não foi possivel acessar a API. Código de erro: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }



        }
        // Função para obter o token de autenticação
        private static async Task<string> GetAuthTokenAsync()
        {
            string token = "";
            string consumerK = ; //add your consumer key here
            string consumerS = ; //add your secret key here embrapa
            string toEncode = $"{consumerK}:{consumerS}";
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(toEncode);
            string base64 = Convert.ToBase64String(bytesToEncode); ; // "dVZZalFaQ0x5V0E5ZDhteWdjdHV0NGJ0NHlVYTpxVlZYcnpoakVoOFdNYTlHRjd0OV9CMVlpazRh";
            string tokenUrl = "https://api.cnptia.embrapa.br/token";
            string authorizationHeader = $@"Basic {base64}"; // Replace with your actual Basic token

            using (HttpClient client = new HttpClient())
            {
                // Creating the data to send in the body (grant_type=client_credentials)
                var data = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

                // Add the Authorization header (Basic token)
                client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

                try
                {
                    // Sending the POST request
                    HttpResponseMessage response = await client.PostAsync(tokenUrl, data);

                    // If the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and return the response content
                        string responseContent = await response.Content.ReadAsStringAsync();
                        TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                        token = tokenResponse.access_token;
                        return token;
                    }
                    else
                    {
                        // Handle the error response
                        throw new Exception($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the request
                    throw new Exception("Error occurred while requesting the token: " + ex.Message);
                }
            }
        }
    }
}
