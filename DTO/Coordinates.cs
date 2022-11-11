namespace SensidevInterview.DTO
{
    // contains the coordinates for every geographical point
    public struct Coordinates
    {
        public readonly string _ServiceKey;
        public readonly double _Lat;
        public readonly double _Lon;
        public Coordinates(string ServiceKey, double Lat, double Lon)
        {
            _ServiceKey = ServiceKey;
            _Lat = Lat;
            _Lon = Lon;
        }
    }
}
