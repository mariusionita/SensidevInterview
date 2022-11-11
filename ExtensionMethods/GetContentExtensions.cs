using SensidevInterview.DTO;

namespace SensidevInterview.ExtensionMethods
{
    public static class GetContentExtensions
    {
        // validates the input to make sure some exceptions are eliminated and gets the content from that specific url
        public static string GetDataFromThirdPartySources(this GetContent coordContent)
        {
            string content = string.Empty;
            try 
            {
                if (coordContent._Client != null && !string.IsNullOrEmpty(coordContent._Url)) 
                {
                    Task<string> t = coordContent._Client.GetStringAsync(coordContent._Url);
                    content = t.Result;
                    coordContent._Client.Dispose();
                    return content;
                }  
            }
            catch(Exception)
            {
                coordContent._Client.Dispose();
            }
            return content;
        }
    }
}
