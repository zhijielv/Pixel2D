using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.Utils;
using LitJson;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Framework.Scripts.Level
{
    public class Level
    {
        [ReadOnly] public LevelType LevelType = LevelType.ludi;
        [OnValueChanged("ChangeSize")] public int Width = 4;
        [OnValueChanged("ChangeSize")] public int Height = 4;
        
        // public float xOffset = .0f;
        // public float yOffset = .0f;
        // public float scale = 0.2f;
        
        public Dictionary<LevelItemType, int> ItemWidget = new Dictionary<LevelItemType, int>();


        public bool showTable = false;
        [TableMatrix(DrawElementMethod = "DrawTableMatrix")] [ReadOnly]
        [ShowIf("showTable")]
        public LevelItemType[,] LevelMap;

        [ListDrawerSettings(Expanded = true)]
        public Dictionary<LevelItemType, List<Sprite>> LevelItem;

        [HideInInspector] public List<GameObject> LevelObj;
        
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
            SetLevelMap();
        }

        public void ChangeSize(int tmp)
        {
            LevelMap = new LevelItemType[Width, Height];
            SetLevelMap();
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
            SetLevelMap();
            LevelItem = new Dictionary<LevelItemType, List<Sprite>>();
            foreach (LevelItemType value in Enum.GetValues(typeof(LevelItemType)))
            {
                if (value == LevelItemType.Maxvalue) return;
                LevelItem.Add(value, new List<Sprite>());
            }
        }

        public void SetLevelMap()
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
            SetLevelRoad();
        }

        public void SetLevelRoad()
        {
            if(ItemWidget.Count == 0) return;
            for (int i = 1; i < Width - 1; i++)
            {
                for (int j = 1; j < Height - 1; j++)
                {
                    LevelMap[i, j] = RandomHelper.GetWidgetRandom(ItemWidget);
                }
            }
        }

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
            SetSize(data.Width, data.Height);
            Enum.TryParse(data.LevelType, out LevelType);
            ItemWidget.Clear();
            foreach (KeyValuePair<string,int> valuePair in data.WidgetDictionary)
            {
                LevelItemType tmpType;
                Enum.TryParse(valuePair.Key, out tmpType);
                ItemWidget.Add(tmpType, valuePair.Value);
            }
            foreach (JsonData data1 in data.RoadList)
            {
                LevelItem[LevelItemType.Road]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.WallList)
            {
                LevelItem[LevelItemType.Wall]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.BoxList)
            {
                LevelItem[LevelItemType.Box]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }

            foreach (JsonData data1 in data.ObstructionList)
            {
                LevelItem[LevelItemType.Obstruction]
                    .Add((Sprite) AssetDatabase.LoadAssetAtPath(data1.ToString(), typeof(Sprite)));
            }
        }

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
    }
}