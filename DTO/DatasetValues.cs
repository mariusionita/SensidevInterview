namespace SensidevInterview.DTO
{
    public struct DatasetValues
    {
        public readonly double _Lat;
        public readonly double _Lon;
        public DatasetValues(double Lat, double Lon)
        {
            _Lat = Lat;
            _Lon = Lon;
        }
    }
}
