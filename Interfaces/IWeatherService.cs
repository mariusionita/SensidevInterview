using SensidevInterview.DTO;

namespace SensidevInterview.Interfaces
{
    public interface IWeatherService
    {
        public IEnumerable<CityTemperature> GetTemperatures();
    }
}
