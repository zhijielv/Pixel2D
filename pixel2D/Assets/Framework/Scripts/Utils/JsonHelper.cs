using System.Threading.Tasks;
using Framework.Scripts.Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        public static async Task<T> JsonReader<T>(string jsonName)
        {
            TextAsset jsonFile = await AddressableManager.Instance.LoadAsset<TextAsset>(jsonName);
            T t = JsonConvert.DeserializeObject<T>(jsonFile.text);
            return t;
        }
    }
}