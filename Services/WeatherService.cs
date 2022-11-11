using Newtonsoft.Json;
using SensidevInterview.DTO;
using SensidevInterview.ExtensionMethods;
using SensidevInterview.Interfaces;
using SensidevInterview.Miscellaneous;
using SensidevInterview.Model;

namespace SensidevInterview.Services
{
    public class WeatherService : IWeatherService
    {        
        private readonly ApiUrls? _settings;
        private HttpClient _httpClient;
        private readonly List<DatasetValues> _data;
        public WeatherService()
        {
            _httpClient = new HttpClient();
            if(Program.settings != null)
                _settings = Program.settings;
            _data = DataSet.Init(); // initialize dataset
        }

        /// <summary>
        /// Get known cities temperatures.
        /// </summary>
        /// <param name="logger">Logger instance used to log any errors.</param>
        /// <returns>A list of cities with temperature information.</returns>
        public IEnumerable<CityTemperature> GetTemperatures()
        {
            try
            {
                List<CityTemperature> resultCollection = new List<CityTemperature>();
                if (_settings != null && _settings.ApiServiceKey != null && _settings.CityNameBaseUrl != null && _settings.CityTemperatureBaseUrl != null) 
                {
                    string cityName = string.Empty;
                    double cityTemp = 0;
                    string result = string.Empty;
                    bool status;
                    foreach (var coordinate in _data)
                    {
                        status = true;
                        GetContent thirdPartyCallContent = new GetContent(_httpClient, new Coordinates(_settings.ApiServiceKey, coordinate._Lat, coordinate._Lon), string.Format(_settings.CityTemperatureBaseUrl, coordinate._Lat, coordinate._Lon, _settings.ApiServiceKey));
                        result = thirdPartyCallContent.GetDataFromThirdPartySources(); // calls extension method
                        if (!string.IsNullOrEmpty(result))                  // deserialize result only if string is not null or empty
                        {
                            var weatherResultObject = JsonConvert.DeserializeObject<dynamic>(result);
                            if (weatherResultObject != null) { cityTemp = weatherResultObject.main.temp - _settings.AbsoluteZeroKelvinTemperature; }
                        }
                        else
                            status = false; // there is an exception ; could not get the value

                        thirdPartyCallContent = new GetContent(_httpClient, new Coordinates(_settings.ApiServiceKey, coordinate._Lat, coordinate._Lon), string.Format(_settings.CityNameBaseUrl, coordinate._Lat, coordinate._Lon, _settings.ApiServiceKey));
                        result = thirdPartyCallContent.GetDataFromThirdPartySources();
                        if (!string.IsNullOrEmpty(result))                  // deserialize result only if string is not null or empty
                        {
                            var geocodingResultObject = JsonConvert.DeserializeObject<dynamic>(result);
                            if (geocodingResultObject != null) { cityName = geocodingResultObject[0].name; }
                        }
                        else
                            status = false; // there is an exception ; could not get the value

                        if (status == true)  // add value into the list only if there are no exceptions occured
                            resultCollection.Add(new CityTemperature(cityTemp, cityName));
                    }
                }
                
                return resultCollection;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"{DateTimeOffset.UtcNow.ToString("O")} - {nameof(GetTemperatures)} - Exception: {ex}");
                _httpClient.Dispose(); // dispose the client if there is an error
                return Array.Empty<CityTemperature>();
            }
        }
    }
}
