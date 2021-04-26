using System.Threading.Tasks;
using Framework.Scripts.Manager;
using LitJson;
using UnityEngine;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        public static async Task<T> JsonReader<T>(string jsonName)
        {
            TextAsset jsonFile = await AddressableManager.Instance.LoadAsset<TextAsset>(jsonName);
            T t = JsonMapper.ToObject<T>(jsonFile.text);
            return t;
        }
    }
}