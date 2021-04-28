﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Editor.Tools;
using Framework.Scripts.Level;
using Framework.Scripts.Manager;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.LevelEditor
{
#if UNITY_EDITOR

    public class LevelEditor : OdinMenuEditorWindow
    {
        private LevelTool _levelTool;

        protected override void OnEnable()
        {
            base.OnEnable();
            _levelTool = new LevelTool();
        }

        [MenuItem("Tools/Level Editor %Q")]
        private static void OpenWindow()
        {
            var window = GetWindow<LevelEditor>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
            window.titleContent = new GUIContent("Level Editor");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new OdinMenuTree(true)
            {
                {"Level Tool", _levelTool},
            };
            return tree;
        }
    }

    public class LevelTool
    {
        [ReadOnly] private string jsonPath = "Assets/Framework/Json/Map.json";
        [OnValueChanged("LoadLevel")] public LevelType levelType = LevelType.ludi;
        [ShowInInspector] public Level Level;

        private List<LeveljsonClass> _levelJsonList;
        private LeveljsonClass _leveljsonClass;

        public LevelTool()
        {
            Level = ReadMapJson(levelType).Result;
        }

        public async void LoadLevel(LevelType levelType)
        {
            Level = await ReadMapJson(levelType);
        }

        public async Task<Level> ReadMapJson(LevelType levelType)
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
                AddAllJson2AddressGroup();
            }

            // _levelJsonList = await JsonHelper.JsonReader<List<LeveljsonClass>>("Map.json");
            StreamReader streamReader = new StreamReader(jsonPath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            _levelJsonList = JsonConvert.DeserializeObject<List<LeveljsonClass>>(json);
            _leveljsonClass = null;
            foreach (LeveljsonClass data in _levelJsonList)
            {
                if (!data.LevelType.Equals(levelType.ToString())) continue;
                _leveljsonClass = data;
                await tmpLevel.GenerateLevelValueFromJson(_leveljsonClass);
                break;
            }

            return tmpLevel;
        }

        public void AddAllJson2AddressGroup()
        {
            AssetDatabase.Refresh();
            List<TextAsset> textAssets = AssetDatabase.FindAssets("t:TextAsset", new[] {"Assets/Framework/Json"})
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
            AddressableAssetsTool.Add2AddressablesGroupsByName(textAssets, "Json");
        }

        [Button]
        public void Save2Json()
        {
            if (_leveljsonClass == null)
            {
                _leveljsonClass = new LeveljsonClass();
                _levelJsonList.Add(_leveljsonClass);
            }

            _leveljsonClass.LevelType = levelType.ToString();
            _leveljsonClass.Height = Level.Height;
            _leveljsonClass.Width = Level.Width;
            _leveljsonClass.WidgetDictionary = new Dictionary<string, int>();
            foreach (KeyValuePair<LevelItemType, int> valuePair in Level.ItemWidget)
            {
                _leveljsonClass.WidgetDictionary.Add(valuePair.Key.ToString(), valuePair.Value);
            }

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
            AddAllJson2AddressGroup();
        }
    }
#endif
}