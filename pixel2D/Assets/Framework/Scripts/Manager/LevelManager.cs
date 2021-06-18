using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Level;
using Framework.Scripts.Level.LevelItem;
using Framework.Scripts.Singleton;
using Framework.Scripts.Utils;
using Pathfinding;
using SRF;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework.Scripts.Manager
{
    public class LevelManager : ManagerSingleton<LevelManager>
    {
        public List<LeveljsonClass> leveljsonClasses;
        public GameObject levelLoaderObj;
        public bool isLevelLoaded = false;
        public Level.Level curLevel = null;
        public bool init = false;
        public List<LevelMap> levelMaps;
        public int curMapIndex = 0;
        public int curLevelIndex = 0;

        public override async Task Init()
        {
            levelMaps = JsonHelper.JsonReader<LevelMap>(Constants.Constants.MapJson);
            leveljsonClasses = JsonHelper.JsonReader<LeveljsonClass>(Constants.Constants.LevelJson);
            transform.localScale =
                new Vector3(Common.LevelManagerScale, Common.LevelManagerScale, Common.LevelManagerScale);
            CreateAStarPath();
            init = true;
        }

        public Level.Level GetLevelByIndex(int mapIndex, int levelIndex)
        {
            Level.Level level = levelMaps[mapIndex].Generate2Level(levelIndex);
            return level;
        }

        public async Task ChangeLevel(object key, LoadSceneMode loadSceneMode)
        {
            await AddressableManager.Instance.LoadScene(key, loadSceneMode);
        }

        // 加载Level数据
        public void InitLevelLoader(Transform mapRoot = null)
        {
            // reset LevelLoader
            if (levelLoaderObj != null) Destroy(levelLoaderObj);
            levelLoaderObj = new GameObject("LevelLoader") {name = "LevelLoader"};
            levelLoaderObj.transform.parent = mapRoot == null ? transform : mapRoot;
            LevelLoader loader =
                levelLoaderObj.GetComponentOrAdd<LevelLoader>();
            loader.Init();
            
            // Create Level
            // curLevel = await _levelMaps[curMapIndex].Generate2Level(curLevelIndex);
            curLevel = GetLevelByIndex(curMapIndex, curLevelIndex);
            loader.level = curLevel;
            loader.CreateLevel();
            
            // reset Pathfinding
            ResetPathfindingSize();
        }

        // 创建 A* 寻路组件
        public void CreateAStarPath()
        {
            GameObject AStarPath = new GameObject("AStarPath");
            AStarPath.transform.SetParent(transform);
            AstarPath astarPath = AStarPath.GetComponentOrAdd<AstarPath>();
            GridGraph gridGraph = astarPath.data.AddGraph(typeof(GridGraph)) as GridGraph;
            // gridGraph 旋转 为2D
            gridGraph.rotation = new Vector3(-90, 270, 90);
            // collision
            gridGraph.collision.use2D = true;
            gridGraph.collision.type = ColliderType.Ray;
            gridGraph.collision.mask = 1 << LayerMask.NameToLayer("Ground");
        }

        // 刷新寻路
        public void ResetPathfindingSize()
        {
            AstarPath.active.data.gridGraph.SetDimensions(curLevel.Width, curLevel.Height,
                Common.TileSize * Common.LevelManagerScale);
            float tmpSize = AstarPath.active.data.gridGraph.nodeSize;
            AstarPath.active.data.gridGraph.center = new Vector3(-1 * tmpSize / 2, tmpSize / 2, 0);
            AstarPath.active.Scan();
        }
    }
}