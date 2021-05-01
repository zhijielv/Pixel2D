using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Level;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Scripts.Manager
{
    public class LevelManager : ManagerSingleton<LevelManager>
    {
        public LevelType levelType = LevelType.ludi;
        public List<LeveljsonClass> leveljsonClasses;
        public GameObject levelLoaderObj;
        public bool isLevelLoaded = false;
        bool init = false;
        public override async Task Init()
        {
            leveljsonClasses = await JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.MapJson);
            transform.localScale = new Vector3(Common.LevelManagerScale, Common.LevelManagerScale, Common.LevelManagerScale);
            init = true;
        }

        public async Task ChangeLevel(object key, LoadSceneMode loadSceneMode)
        {
            await AddressableManager.Instance.LoadScene(key, loadSceneMode);
        }

        public async Task LoadLevel(Transform mapRoot = null)
        {
            if(levelLoaderObj != null) Destroy(levelLoaderObj);
            levelLoaderObj = new GameObject("LevelLoader"); 
            levelLoaderObj.transform.parent = mapRoot == null ? transform : mapRoot;
            LevelLoader loader = (LevelLoader) Constants.Constants.AddOrGetComponent(levelLoaderObj, typeof(LevelLoader));
            await loader.Init();
            await loader.LoadLevel(levelType);
            loader.CreateLevel();
        }

        public async Task<Level.Level> GetLevel(LevelType levelType = LevelType.ludi)
        {
            Level.Level level = new Level.Level();
            await level.GenerateLevelValueFromJson(await GetLeveljsonClass(levelType));
            return level;
        } 

        public async Task<LeveljsonClass> GetLeveljsonClass(LevelType levelType = LevelType.ludi)
        {
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            leveljsonClasses ??= await JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.MapJson);
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