using System.IO;
using System.Threading.Tasks;
using LitJson;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        public static async Task<T> JsonReader<T>(string jsonName)
        {
            TextAsset jsonFile = await Addressables.LoadAssetAsync<TextAsset>(jsonName).Task;
            T t = JsonMapper.ToObject<T>(jsonFile.text);
            return t;
        }
    }
}