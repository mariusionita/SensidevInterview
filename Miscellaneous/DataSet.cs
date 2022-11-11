using SensidevInterview.DTO;

namespace SensidevInterview.Miscellaneous
{
    public static class DataSet
    {
        private static List<DatasetValues>? _Data;
        public static List<DatasetValues> Init() 
        {
            if (_Data == null)
                _Data = new List<DatasetValues>() { new DatasetValues(45.764479, 21.228447),
                                                    new DatasetValues(44.431514, 26.103138),
                                                    new DatasetValues(47.153058,27.590374),
                                                    new DatasetValues(46.761286, 23.580379),
                                                    new DatasetValues(45.656547, 25.615596),
                                                    new DatasetValues(44.181662, 28.635918)
                                                   };
            return _Data;
        }
    }
}
