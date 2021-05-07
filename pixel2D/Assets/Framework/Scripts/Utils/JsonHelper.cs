using System.Threading.Tasks;
using Framework.Scripts.Manager;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Utils
{
    public class JsonHelper
    {
        public static async Task<T> JsonReader<T>(string jsonName)
        {
#if UNITY_EDITOR
            jsonName = "Assets/Framework/Json/" + jsonName + ".json";
            TextAsset jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(jsonName);
#else
            TextAsset jsonFile = await AddressableManager.Instance.LoadAsset<TextAsset>(jsonName);
#endif
            T t = JsonConvert.DeserializeObject<T>(jsonFile.text);
            return t;
        }
    }
}