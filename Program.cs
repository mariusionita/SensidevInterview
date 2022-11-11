using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensidevInterview.DTO;
using SensidevInterview.Interfaces;

public class WeatherService : IWeatherService
{
    private const string _serviceKey = "a2dcdf88e1bde658110f086ad638bf4a"; // put this into appsettings.json file
    private static (double Lat, double Lon)[] _knownCoordinates = new (double Lat, double Lon)[] { new (45.764479, 21.228447), new (44.431514, 26.103138), new (47.153058, 27.590374), new (46.761286, 23.580379), new (45.656547, 25.615596), new (44.181662, 28.635918) };
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
            List<CityTemperature> resultCollection = new List<CityTemperature>();
            foreach (var coordinate in _knownCoordinates)
            {
                // create extension methods for _httpClient.GetStringAsync methods and put as parameter the coordinates
                // put the base url (https://api.openweathermap.org/data/2.5/weather?lat=) into config file appsettings.json
                var result = _httpClient.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={coordinate.Lat}&lon={coordinate.Lon}&appid={_serviceKey}").Result;
                //check if _httpClient.GetStringAsync returned succesfully and if result is not null;
                var weatherResultObject = JsonConvert.DeserializeObject<dynamic>(result);
                if (weatherResultObject != null) {  double cityTemp = weatherResultObject.main.temp - 273.15;}
                result = _httpClient.GetStringAsync($"http://api.openweathermap.org/geo/1.0/reverse?lat={coordinate.Lat}&lon={coordinate.Lon}&appid={_serviceKey}").ConfigureAwait(false).GetAwaiter().GetResult();
                var geocodingResultObject = JsonConvert.DeserializeObject<dynamic>(result);
                if (geocodingResultObject != null) { string cityName = geocodingResultObject[0].name; }
                // add new item to the list only if cityTemp, cityName are not null
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

public class Program
{
    public static void Main(string[] args)
    {   // add this config section into a startup file
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthorization();
        builder.Services.AddScoped<IWeatherService, WeatherService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        Random rnd = new Random();

        app.MapGet("/weather", (HttpContext httpContext, [FromServices] IWeatherService weatherService) =>
        {
            var apiResult = weatherService.GetTemperatures();
            var hotCitiesCount = apiResult.Where(x => x.Temperature > 27.0).Count();
            var coldCitiesCount = apiResult.Where(x => x.Temperature < 15.0).Count();
            var sampleCity = apiResult.Count() > 0 ? apiResult.ElementAt(rnd.Next(0, apiResult.Count())) : new CityTemperature(0, "NoData");
            return new ResponseObject(hotCitiesCount, coldCitiesCount, apiResult.ToArray(), sampleCity);
        })
        .WithName("GetWeather");

        app.Run();
    }
}

