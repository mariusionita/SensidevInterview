namespace SensidevInterview.DTO
{
    public struct GetContent
    {
        public readonly HttpClient _Client;
        public readonly Coordinates _Coord;
        public string _Url;
        public GetContent(HttpClient Client, Coordinates Coord, string Url)
        {
            _Client= Client;    
            _Coord= Coord;
            _Url = Url;
        }
    }
}
