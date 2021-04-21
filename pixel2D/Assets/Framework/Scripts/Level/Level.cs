using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Framework.Scripts.Level
{
    public enum LevelType
    {
        ludi,
        yanjiang,
        xueshan,
    }
    public enum LevelItemType
    {
        Road = 0,
        Wall,
        Box,
        Door,
        Obstruction,
    }
    
    public class LeveljsonClass
    {
        public string LevelType;
        public int Width;
        public int Height;
        public List<string> RoadList = new List<string>();
        public List<string> WallList = new List<string>();
        public List<string> BoxList = new List<string>();
        public List<string> DoorList = new List<string>();
        public List<string> ObstructionList = new List<string>();
    }
    public class Level
    {
        public LevelType LevelType;
        public int Width;
        public int Height;

        [TableMatrix(DrawElementMethod = "DrawTableMatrix")] [ReadOnly]
        public int[,] LevelMap;

        public Dictionary<LevelItemType, List<Sprite>> LevelItem;

        [HideInInspector] public List<GameObject> LevelObj;

        public Level()
        {
            SetSize();
        }

        // [Button]
        public void ClearLevelObj()
        {
            for (var i = 0; i < LevelObj.Count; i++)
            {
                Object.DestroyImmediate(LevelObj[i]);
            }
        }
        public void SetSize(int width = 0, int height = 0)
        {
            if (LevelObj != null) ClearLevelObj();
            LevelObj = new List<GameObject>();
            if (width != 0)
                Width = width;
            if (height != 0)
                Height = height;
            LevelMap = new int[Width, Height];
            LevelItem = new Dictionary<LevelItemType, List<Sprite>>();
            foreach (LevelItemType value in Enum.GetValues(typeof(LevelItemType)))
            {
                LevelItem.Add(value, new List<Sprite>());
            }
            // for (int i = 0; i < 2; i++)
            // {
            //     string path = "Assets/Art/yuanqiqishiExportAll/Sprite/" + LevelMapName + "1/" + LevelMapName +
            //                   (i + 1) + ".png";
            //     Sprite sprite =
            //         (Sprite) AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            //     LevelItem.Add(i, sprite);
            // }
        }

        static int DrawTableMatrix(Rect rect, int value)
        {
            EditorGUI.LabelField(rect, value.ToString());
            return value;
        }
    }
}