using Newtonsoft.Json;
using static AppMeteo_3._0.Models.ApiMeteoModel;

namespace AppMeteo_3._0.Services
{
    public class ApiServices
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ApiServices(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<List<string>> GetMeteo(string cittaDaCercare)
        {
            string apiKey = _configuration["apiKey:Default"];
            string locationUrl = _configuration["locationUrls:urlLocationKey"];
            string meteoUrl = _configuration["locationUrls:urlMeteo"];

            // Sostituisci il segnaposto {apiKey} con il valore reale dell'API key
            locationUrl = $"{locationUrl}&q={cittaDaCercare}&language=it-it";
            locationUrl = locationUrl.Replace("{apiKey}", apiKey);




            try
            {
                // Effettua la richiesta per ottenere le informazioni sulla posizione
                HttpResponseMessage locationResponse = await _httpClient.GetAsync(locationUrl);
                locationResponse.EnsureSuccessStatusCode(); // Lanciata un'eccezione se la risposta non è di successo

                string locationJsonResponse = await locationResponse.Content.ReadAsStringAsync();
                LocationResponse[] locationResult = JsonConvert.DeserializeObject<LocationResponse[]>(locationJsonResponse);

                if (locationResult.Length == 0)
                {
                    throw new Exception("Posizione non trovata.");
                }

                string locationKey = locationResult[0].Key;

                meteoUrl = meteoUrl.Replace("{apiKey}", apiKey);
                meteoUrl = meteoUrl.Replace("{locationKey}", locationKey);

                // Effettua la richiesta per ottenere le informazioni meteorologiche
                HttpResponseMessage meteoResponse = await _httpClient.GetAsync(meteoUrl);

                // eccezzione se non va a buon fine 
                meteoResponse.EnsureSuccessStatusCode();

                string meteoJsonResponse = await meteoResponse.Content.ReadAsStringAsync();
                JsonResponse[] jsonResponse = JsonConvert.DeserializeObject<JsonResponse[]>(meteoJsonResponse);

                if (jsonResponse.Length == 0)
                {
                    throw new Exception("Dati meteo non trovati.");
                }

                Temperature temperatura = jsonResponse[0].Temperature;
                string meteo = jsonResponse[0].WeatherText;

                var result = new List<string>
                {
                    $"{temperatura.Metric.Value} {temperatura.Metric.Unit}",
                    meteo
                };

                return result;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Errore di richiesta HTTP: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Errore durante il recupero dei dati meteo: {ex.Message}");
            }
        }
    }
}
