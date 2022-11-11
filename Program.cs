using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SensidevInterview.DTO;
using SensidevInterview.ExtensionMethods;
using SensidevInterview.Interfaces;




public class Program
{
    public static void Main(string[] args)
    {   
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

