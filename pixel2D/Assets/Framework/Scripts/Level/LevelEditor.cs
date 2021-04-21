using System;
using System.Collections.Generic;
using System.IO;
using HutongGames.PlayMaker;
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
        [ReadOnly] public string jsonPath = "Assets/Framework/Json/";
        [InlineButton("LoadLevel")] public LevelType levelType;
        [ShowInInspector] public Level level;
        
        private List<LeveljsonClass> LevelJsonlist;
        private LeveljsonClass _leveljsonClass;
        private bool hideSave = true;

        public LevelTool()
        {
            hideSave = true;
            LoadLevel();
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
        
        public void LoadLevel()
        {
            if (!File.Exists(jsonPath + "Map.json"))
            {
                Debug.Log("创建 Map.json at " + jsonPath);
                FileStream fileStream = File.Create(jsonPath + "Map.json");
                StreamWriter streamWriter = new StreamWriter(fileStream);
                streamWriter.Write("[]");
                streamWriter.Close();
                fileStream.Close();
                level = new Level();
                return;
            }

            StreamReader streamReader = new StreamReader(jsonPath + "Map.json");
            string json = streamReader.ReadToEnd();
            streamReader.Close();
            LevelJsonlist = JsonMapper.ToObject<List<LeveljsonClass>>(json);
            foreach (LeveljsonClass data in LevelJsonlist)
            {
                if (!data.LevelType.Equals(levelType.ToString())) continue;
                _leveljsonClass = data;
                GetLevelValue(_leveljsonClass);
                break;
            }
            hideSave = false;
        }
        
        [Sirenix.OdinInspector.HideIf("hideSave")]
        public void Save2Json()
        {
            if (level != null) return;
            _leveljsonClass ??= new LeveljsonClass();
            LevelJsonlist.Add(_leveljsonClass);
            
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
            StreamWriter streamWriter = new StreamWriter(jsonPath + "Map.json") {AutoFlush = true};
            streamWriter.Write(json);
            streamWriter.Close();
        }
    }
}