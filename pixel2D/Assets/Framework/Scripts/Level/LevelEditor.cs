using System;
using System.Collections.Generic;
using System.IO;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Scripts.Level
{
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
                {"Level Tool", _levelTool}
            };
            return tree;
        }
    }

    public class LevelTool
    {
        [ReadOnly] private const string jsonPath = "Assets/Framework/Json/" + "Map.json";
        [OnValueChanged("LoadLevel")] public LevelType levelType = LevelType.ludi;
        [ShowInInspector] public Level level;
        
        private List<LeveljsonClass> LevelJsonlist;
        private LeveljsonClass _leveljsonClass;

        public LevelTool()
        {
            LoadLevel(levelType);
        }

        public void GetLevelValue(LeveljsonClass data)
        {
            level = new Level
            {
                Width = data.Width,
                Height = data.Height
            };
            Enum.TryParse(data.LevelType, out level.LevelType);
            foreach (JsonData data1 in data.RoadList)
            {
                level.LevelItem[LevelItemType.Road]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.WallList)
            {
                level.LevelItem[LevelItemType.Wall]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.BoxList)
            {
                level.LevelItem[LevelItemType.Box]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.DoorList)
            {
                level.LevelItem[LevelItemType.Door]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.ObstructionList)
            {
                level.LevelItem[LevelItemType.Obstruction]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }
        }
        
        public void LoadLevel(LevelType levelType)
        {
            if (!File.Exists(jsonPath))
            {
                Debug.Log("创建 Map.json at " + jsonPath);
                FileStream fileStream = File.Create(jsonPath);
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[]");
                streamWriter.Close();
                fileStream.Close();
            }
            
            level = new Level {LevelType = levelType};
            StreamReader streamReader = new StreamReader(jsonPath);
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            LevelJsonlist = JsonMapper.ToObject<List<LeveljsonClass>>(json);
            _leveljsonClass = null;
            foreach (LeveljsonClass data in LevelJsonlist)
            {
                if (!data.LevelType.Equals(levelType.ToString())) continue;
                _leveljsonClass = data;
                GetLevelValue(_leveljsonClass);
                break;
            }
        }
        
        [Button]
        public void Save2Json()
        {
            if (_leveljsonClass == null)
            {
                _leveljsonClass = new LeveljsonClass();
                LevelJsonlist.Add(_leveljsonClass);
            }

            _leveljsonClass.LevelType = levelType.ToString();
            _leveljsonClass.Height = level.Height;
            _leveljsonClass.Width = level.Width;
            foreach (KeyValuePair<LevelItemType, List<Sprite>> keyValuePair in level.LevelItem)
            {
                switch (keyValuePair.Key)
                {
                    case LevelItemType.Road:
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.RoadList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Wall:
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.WallList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Box:
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.BoxList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Door:
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.DoorList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                    case LevelItemType.Obstruction:
                        foreach (Sprite sprite in keyValuePair.Value)
                            _leveljsonClass.ObstructionList.Add(AssetDatabase.GetAssetPath(sprite));
                        break;
                }
            }

            string json = JsonMapper.ToJson(LevelJsonlist);
            StreamWriter streamWriter = new StreamWriter(jsonPath) {AutoFlush = true};
            streamWriter.Write(json);
            streamWriter.Close();
            _leveljsonClass = null;
            AssetDatabase.Refresh();
        }
    }
}