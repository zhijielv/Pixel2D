using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Level.LevelItem;
using Framework.Scripts.Manager;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Scripts.Level
{
    public class Level
    {
        [ReadOnly] public string LevelName;
        [ReadOnly] public LevelType LevelType = LevelType.ludi;
        [OnValueChanged("ChangeSize")] public int Width = 4;
        [OnValueChanged("ChangeSize")] public int Height = 4;
        
        // public float xOffset = .0f;
        // public float yOffset = .0f;
        // public float scale = 0.2f;
        
        // public Dictionary<LevelItemType, int> ItemWidget = new Dictionary<LevelItemType, int>();


        public bool showTable = false;
#if UNITY_EDITOR
        [TableMatrix(DrawElementMethod = "DrawTableMatrix")] [ReadOnly]
        [ShowIf("showTable")]  
#endif
        public LevelItemType[,] LevelMap;

        [ListDrawerSettings(Expanded = true)]
        public Dictionary<LevelItemType, List<Sprite>> LevelItem;

        public List<GameObject> LevelObj;
        
        [Button]
        public void ClearLevelObj()
        {
            for (var i = 0; i < LevelObj.Count; i++)
            {
                Object.DestroyImmediate(LevelObj[i]);
            }
        }

        public Level()
        {
            SetSize();
            SetLevelWall();
        }

        public void ChangeSize(int tmp)
        {
            LevelMap = new LevelItemType[Width, Height];
            SetLevelWall();
        }

        public void SetSize(int width = 4, int height = 4)
        {
            if (LevelObj != null) ClearLevelObj();
            LevelObj = new List<GameObject>();
            Width = width;
            Height = height;
            // xOffset = Random.Range(0, 5f);
            // yOffset = Random.Range(0, 5f);
            LevelMap = new LevelItemType[Width, Height];
            SetLevelWall();
            LevelItem = new Dictionary<LevelItemType, List<Sprite>>();
            foreach (LevelItemType value in Enum.GetValues(typeof(LevelItemType)))
            {
                if (value == LevelItemType.Maxvalue) return;
                LevelItem.Add(value, new List<Sprite>());
            }
        }

        public void SetLevelWall()
        {
            // 设置墙体
            for (int i = 0; i < LevelMap.GetLength(0); i++)
            {
                LevelMap[i, 0] = LevelItemType.Wall;
                LevelMap[i, Height - 1] = LevelItemType.Wall;
            }

            for (int i = 0; i < LevelMap.GetLength(1); i++)
            {
                LevelMap[0, i] = LevelItemType.Wall;
                LevelMap[Width - 1, i] = LevelItemType.Wall;
            }

            // 柏林噪声 随机生成关卡内容
            // CreatePerlinNoiseMap(0f);
            // SetLevelRoad();
        }

        // public void SetLevelRoad()
        // {
        //     if(ItemWidget.Count == 0) return;
        //     for (int i = 1; i < Width - 1; i++)
        //     {
        //         for (int j = 1; j < Height - 1; j++)
        //         {
        //             LevelMap[i, j] = RandomHelper.GetWidgetRandom(ItemWidget);
        //         }
        //     }
        // }

        // 柏林噪声，效果不合适
        // public void CreatePerlinNoiseMap(float tmp)
        // {
        //     for (int i = 1; i < Width - 1; i++)
        //     {
        //         for (int j = 1; j < Height - 1; j++)
        //         {
        //             float x = i / (Width * scale) + xOffset;
        //             float y = j / (Height * scale) + yOffset;
        //             LevelMap[i, j] = (LevelItemType) (Mathf.PerlinNoise(x, y) * ((double) LevelItemType.Maxvalue));
        //         }
        //     }
        // }

        public void GenerateLevelValueFromJson(LeveljsonClass data)
        {
            // SetSize(data.Width, data.Height);
            LevelType = data.LevelType;
            // Enum.TryParse(data.LevelType, out LevelType);
            // ItemWidget.Clear();
            // foreach (KeyValuePair<string,int> valuePair in data.WidgetDictionary)
            // {
                // LevelItemType tmpType;
                // Enum.TryParse(valuePair.Key, out tmpType);
                // ItemWidget.Add(tmpType, valuePair.Value);
            // }
            
            foreach (string data1 in data.RoadList)
            {
#if UNITY_EDITOR
                LevelItem[LevelItemType.Road]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1, typeof(Sprite)));
#else
                string path = Constants.Constants.ReplaceString(data1.ToString(), "Assets/Art/", "");
                Sprite sprite = AddressableManager.Instance.LoadAsset<Sprite>(path);
                LevelItem[LevelItemType.Road]
                    .Add(sprite);
#endif
            }

            foreach (string data1 in data.WallList)
            {
#if UNITY_EDITOR
                LevelItem[LevelItemType.Wall]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1, typeof(Sprite)));
#else
                string path = Constants.Constants.ReplaceString(data1.ToString(), "Assets/Art/", "");
                Sprite sprite = AddressableManager.Instance.LoadAsset<Sprite>(path);
                LevelItem[LevelItemType.Wall]
                    .Add(sprite);
#endif
            }

            foreach (string data1 in data.BoxList)
            {
#if UNITY_EDITOR
                LevelItem[LevelItemType.Box]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1, typeof(Sprite)));
#else
                string path = Constants.Constants.ReplaceString(data1.ToString(), "Assets/Art/", "");
                Sprite sprite = AddressableManager.Instance.LoadAsset<Sprite>(path);
                LevelItem[LevelItemType.Box]
                    .Add(sprite);
#endif
            }

            foreach (string data1 in data.ObstructionList)
            {
#if UNITY_EDITOR
                LevelItem[LevelItemType.Obstruction]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1, typeof(Sprite)));
#else
                string path = Constants.Constants.ReplaceString(data1.ToString(), "Assets/Art/", "");
                Sprite sprite = AddressableManager.Instance.LoadAsset<Sprite>(path);
                LevelItem[LevelItemType.Obstruction]
                    .Add(sprite);
#endif
            }
        }
#if UNITY_EDITOR
        static LevelItemType DrawTableMatrix(Rect rect, LevelItemType value)
        {
            switch (value)
            {
                case LevelItemType.Road : EditorGUI.DrawRect(rect.Padding(3), new Color(1, 0, 0));
                    break;
                case LevelItemType.Wall : EditorGUI.DrawRect(rect.Padding(3), new Color(0, 1, 0));
                    break;
                case LevelItemType.Box : EditorGUI.DrawRect(rect.Padding(3), new Color(0, 0, 1));
                    break;
                case LevelItemType.Obstruction : EditorGUI.DrawRect(rect.Padding(3), new Color(0, 1, 1));
                    break;
                default: EditorGUI.DrawRect(rect.Padding(3), new Color(1, 1, 1));
                    break;
            }
            return value;
        }
#endif
        public LevelType GetLevelType()
        {
            return LevelType;
        }
        
        public Level GetLevel(LevelType levelType = LevelType.ludi)
        {
            Level level = new Level();
            level.GenerateLevelValueFromJson(GetLeveljsonClass(levelType));
            return level;
        }

        public LeveljsonClass GetLeveljsonClass(LevelType levelType = LevelType.ludi)
        {
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            List<LeveljsonClass> leveljsonClasses = new List<LeveljsonClass>();
            JsonHelper.JsonReader(out leveljsonClasses, Constants.Constants.LevelJson);
            foreach (LeveljsonClass leveljsonClass in leveljsonClasses)
            {
                if (leveljsonClass.LevelType != levelType) continue;
                tmpLeveljsonClass = leveljsonClass;
            }

            return tmpLeveljsonClass;
        }
    }
}