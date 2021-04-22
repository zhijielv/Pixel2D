using System.IO;
using LitJson;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        public static T JsonReader<T>(string jsonPath)
        {
            StreamReader streamReader = new StreamReader(jsonPath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            T t = JsonMapper.ToObject<T>(json);
            return t;
        }
    }
}