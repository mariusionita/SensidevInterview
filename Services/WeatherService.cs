using Newtonsoft.Json;
using SensidevInterview.DTO;
using SensidevInterview.ExtensionMethods;
using SensidevInterview.Interfaces;

namespace SensidevInterview.Services
{
    public class WeatherService : IWeatherService
    {
        private const string _serviceKey = "a2dcdf88e1bde658110f086ad638bf4a"; // put this into appsettings.json file
        private static (double Lat, double Lon)[] _knownCoordinates = new (double Lat, double Lon)[] { new(45.764479, 21.228447), new(44.431514, 26.103138), new(47.153058, 27.590374), new(46.761286, 23.580379), new(45.656547, 25.615596), new(44.181662, 28.635918) };
        private HttpClient _httpClient;

        public WeatherService()
        {
            _httpClient = new HttpClient();
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
                string cityName = string.Empty;
                double cityTemp = 0;
                string result = string.Empty;
                bool status;
                List<CityTemperature> resultCollection = new List<CityTemperature>();
                foreach (var coordinate in _knownCoordinates)
                {
                    status = true;
                    GetContent thirdPartyCallContent = new GetContent(_httpClient, new Coordinates(_serviceKey, coordinate.Lat, coordinate.Lon), "url");
                    result = thirdPartyCallContent.GetDataFromThirdPartySources(); // calls extension method
                    if (!string.IsNullOrEmpty(result))                  // deserialize result only if string is not null or empty
                    {
                        var weatherResultObject = JsonConvert.DeserializeObject<dynamic>(result);
                        if (weatherResultObject != null) { cityTemp = weatherResultObject.main.temp - 273.15; }
                    }
                    else
                        status = false; // there is an exception ; could not get the value

                    thirdPartyCallContent = new GetContent(_httpClient, new Coordinates(_serviceKey, coordinate.Lat, coordinate.Lon), "url");
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
