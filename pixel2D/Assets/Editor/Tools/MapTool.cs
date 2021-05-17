/*
** Created by fengling
** DateTime:    2021-05-10 11:49:40
** Description: TODO 
*/

using System.Collections.Generic;
using System.IO;
using Framework.Scripts.Constants;
using Framework.Scripts.Level;
using Framework.Scripts.Utils;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;

namespace Editor.Tools
{
    public class MapTool
    {
        [ReadOnly] public string jsonPath = "Assets/Framework/Json/" + Constants.MapJson + ".json";
        [ShowInInspector]
        public List<LevelMap> Maps;

        public MapTool()
        {
            Maps = JsonHelper.ReadOrCreateJson<LevelMap>(Constants.MapJson);
        }

        [Button("保存地图数据", ButtonSizes.Large)]
        public void SaveMap2Json()
        {
            string json = JsonConvert.SerializeObject(Maps);
            StreamWriter streamWriter = new StreamWriter(jsonPath) {AutoFlush = true};
            streamWriter.Write(json);
            streamWriter.Close();
            AssetDatabase.Refresh();
            AddressableAssetsTool.AddAllJson2AddressGroup();
        }
    }
}