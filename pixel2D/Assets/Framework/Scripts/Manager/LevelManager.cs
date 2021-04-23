using System.Collections.Generic;
using Framework.Scripts.Level;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class LevelManager : ManagerSingleton<LevelManager>
    {
        public LevelType levelType = LevelType.ludi;
        public List<LeveljsonClass> leveljsonClasses;
        public GameObject levelLoaderObj;
        public LevelManager()
        {
            leveljsonClasses = JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.JsonPath);
        }

        public void LoadLevel(Transform mapRoot = null)
        {
            if(levelLoaderObj != null) Destroy(levelLoaderObj);
            levelLoaderObj = new GameObject("LevelLoader");
            LevelLoader loader = (LevelLoader) Constants.Constants.AddOrGetComponent(levelLoaderObj, typeof(LevelLoader));
            levelLoaderObj.transform.parent = mapRoot == null ? transform : mapRoot;

            loader.LoadLevel(levelType);
            loader.CreateLevel();
        }

        public Level.Level GetLevel(LevelType levelType = LevelType.ludi)
        {
            Level.Level level = new Level.Level();
            level.GenerateLevelValueFromJson(GetLeveljsonClass(levelType));
            return level;
        } 

        public LeveljsonClass GetLeveljsonClass(LevelType levelType = LevelType.ludi)
        {
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            foreach (LeveljsonClass leveljsonClass in leveljsonClasses)
            {
                if (!leveljsonClass.LevelType.Equals(levelType.ToString())) continue;
                tmpLeveljsonClass = leveljsonClass;
            }

            return tmpLeveljsonClass;
        }
    }
    
    public enum LevelType
    {
        ludi,
        yanjiang,
        xueshan,
    }

    public enum LevelItemType
    {
        Wall = 0,
        Road,
        Box,
        Obstruction,
        Maxvalue,
    }

    public class LeveljsonClass
    {
        public string LevelType;
        public int Width;
        public int Height;
        public List<string> RoadList = new List<string>();
        public List<string> WallList = new List<string>();
        public List<string> BoxList = new List<string>();
        public List<string> ObstructionList = new List<string>();
        public Dictionary<string, int> WidgetDictionary = new Dictionary<string, int>();
    }
}