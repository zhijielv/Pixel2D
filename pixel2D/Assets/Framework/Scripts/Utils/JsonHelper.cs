using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Framework.Scripts.Level.LevelItem;
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
        
        /// <summary>
        /// 格式化读取某种类型的Json列表
        /// </summary>
        /// <param name="jsonName">json文件名（不带后缀）</param>
        /// <typeparam name="T">要获取的类</typeparam>
        public static async Task<List<T>> ReadOrCreateJson<T>(string jsonName) where T : class, new()
        {
            string json;
            if (Application.isPlaying)
            {
                TextAsset textAsset = AddressableManager.Instance.LoadAsset<TextAsset>(jsonName);
                json = textAsset.text;
                return JsonConvert.DeserializeObject<List<T>>(json);    
            }

            string jsonPath = $"Assets/Framework/Json/{jsonName}.json";
            if (!File.Exists(jsonPath))
            {
                Debug.Log($"创建 {jsonName}.json at " + jsonPath);
                FileStream fileStream = File.Create(jsonPath);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[]");
                streamWriter.Close();
                fileStream.Close();
            }

            var streamReader = new StreamReader(jsonPath);
            json = streamReader.ReadToEnd();
            streamReader.Close();
            return JsonConvert.DeserializeObject<List<T>>(json);
        }

    }
}