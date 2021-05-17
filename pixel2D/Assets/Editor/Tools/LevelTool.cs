/*
** Created by fengling
** DateTime:    2021-04-30 14:16:23
** Description: TODO 
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Level;
using Framework.Scripts.Level.LevelItem;
using Framework.Scripts.Manager;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Editor.Tools
{
    public class LevelTool
    {
        [ReadOnly] private string jsonPath = "Assets/Framework/Json/" + Constants.LevelJson + ".json";
        [OnValueChanged("LoadLevel")] public LevelType levelType = LevelType.ludi;
        [ShowInInspector] public Level Level;

        private List<LeveljsonClass> _levelJsonList;
        private LeveljsonClass _leveljsonClass;

        public LevelTool()
        {
            Level = ReadLevelJson(levelType).Result;
        }

        public async void LoadLevel(LevelType levelType)
        {
            Level = await ReadLevelJson(levelType);
        }

        public async Task<Level> ReadLevelJson(LevelType levelType)
        {
            Level tmpLevel = new Level {LevelType = levelType};
            if (!File.Exists(jsonPath))
            {
                Debug.Log("创建 Map.json at " + jsonPath);
                FileStream fileStream = File.Create(jsonPath);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[]");
                streamWriter.Close();
                fileStream.Close();
                AddressableAssetsTool.AddAllJson2AddressGroup();
            }

            // _levelJsonList = await JsonHelper.JsonReader<List<LeveljsonClass>>("Map.json");
            StreamReader streamReader = new StreamReader(jsonPath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            _levelJsonList = JsonConvert.DeserializeObject<List<LeveljsonClass>>(json);
            _leveljsonClass = null;
            foreach (LeveljsonClass data in _levelJsonList)
            {
                if (data.LevelType != levelType) continue;
                _leveljsonClass = data;
                tmpLevel.GenerateLevelValueFromJson(_leveljsonClass);
                break;
            }

            return tmpLevel;
        }

        [Button]
        public void Save2Json()
        {
            if (_leveljsonClass == null)
            {
                _leveljsonClass = new LeveljsonClass();
                _levelJsonList.Add(_leveljsonClass);
            }

            // _leveljsonClass.LevelType = levelType.ToString();
            _leveljsonClass.LevelType = levelType;
            // _leveljsonClass.Height = Level.Height;
            // _leveljsonClass.Width = Level.Width;
            // _leveljsonClass.WidgetDictionary = new Dictionary<string, int>();
            // foreach (KeyValuePair<LevelItemType, int> valuePair in Level.ItemWidget)
            // {
                // _leveljsonClass.WidgetDictionary.Add(valuePair.Key.ToString(), valuePair.Value);
            // }

            foreach (KeyValuePair<LevelItemType, List<Sprite>> keyValuePair in Level.LevelItem)
            {
                switch (keyValuePair.Key)
                {
                    case LevelItemType.Road:
                        _leveljsonClass.RoadList.Clear();
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.RoadList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Wall:
                        _leveljsonClass.WallList.Clear();
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.WallList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Box:
                        _leveljsonClass.BoxList.Clear();
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.BoxList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Obstruction:
                        _leveljsonClass.ObstructionList.Clear();
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.ObstructionList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                }
            }

            string json = JsonConvert.SerializeObject(_levelJsonList);
            StreamWriter streamWriter = new StreamWriter(jsonPath) {AutoFlush = true};
            streamWriter.Write(json);
            streamWriter.Close();
            _leveljsonClass = null;
            AssetDatabase.Refresh();
            AddressableAssetsTool.AddAllJson2AddressGroup();
        }
    }
}