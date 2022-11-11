namespace SensidevInterview.DTO
{
    public record ResponseObject(int NumberOfColdCities, int NumberOfHotCities, CityTemperature[] Cities, CityTemperature SampleCity);
}
