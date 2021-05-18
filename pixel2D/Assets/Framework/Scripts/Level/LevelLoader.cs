using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Level.LevelItem;
using Framework.Scripts.Manager;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject imagePrefab;
        public Transform mapRoot;
#if UNITY_EDITOR
        [LabelText("地图id")] public int mapIndex = 0;
        [LabelText("关卡id")] public int levelIndex = 0;
#endif
        [ReadOnly] public string levelName;
        [ReadOnly] public LevelType levelType;
        [ShowInInspector] public Level level;

        public void Init()
        {
            imagePrefab = ObjectManager.Instance.LoadUnit();
            mapRoot = transform;
            ResetTransform();
        }

        private void ResetTransform()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = new Quaternion(0, 0, 0, 1);
            transform.localScale = Vector3.one;
        }
#if UNITY_EDITOR
        private List<LevelMap> _levelMaps;
        // 仅Inspector显示用
        // [OnInspectorInit]
        public async Task InspectorInit()
        {
            imagePrefab = await Addressables.LoadAssetAsync<GameObject>(Constants.Constants.ObjectUnit).Task;
            mapRoot = transform;
            _levelMaps = JsonHelper.ReadOrCreateJson<LevelMap>(Constants.Constants.MapJson);
        }
        
        [Button("加载关卡", ButtonSizes.Large)]
        public async void LoadLevelInspector()
        {
            await InspectorInit();
            if (mapIndex >= _levelMaps.Count)
            {
                mapIndex = _levelMaps.Count;
                return;
            }

            if (levelIndex >= _levelMaps[mapIndex].Levels.Count)
            {
                levelIndex = _levelMaps[mapIndex].Levels.Count;
                return;
            }
            // 读取map
            LevelMap levelMap = _levelMaps[mapIndex];
            level = new Level();
            LevelMapJsonClass levelMapJsonClass = levelMap[levelIndex];
            level.LevelName = levelMapJsonClass.LevelName;
            level.LevelType = levelMapJsonClass.LevelType;
            level.SetSize(levelMapJsonClass.Width, levelMapJsonClass.Height);

            // 读取level
            levelName = level.LevelName;
            levelType = level.LevelType;
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            var list = JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.LevelJson);
            foreach (var leveljsonClass in list)
            {
                if (leveljsonClass.LevelType != levelType) continue;
                tmpLeveljsonClass = leveljsonClass;
            }
            
            level.GenerateLevelValueFromJson(tmpLeveljsonClass);

            level.SetLevelWall();
            levelMapJsonClass.SetLevelRoad(level);
            ClearChildren();
            CreateLevel();
        }
        
#endif
        // 创建关卡到游戏中
        [HorizontalGroup("Button")]
        [Button("创建关卡", ButtonSizes.Large)]
        public void CreateLevel()
        {
            level.ClearLevelObj();
            // level.ChangeSize(0);
            int tmpObjName = 0;
            for (int i = 0; i < level.LevelMap.GetLength(0); i++)
            {
                for (int j = 0; j < level.LevelMap.GetLength(1); j++)
                {
                    List<Sprite> sprites = level.LevelItem[level.LevelMap[i, j]] ?? level.LevelItem[LevelItemType.Road];
                    Sprite sprite = Constants.RandomHelper.GetRandomValueFromList(sprites, 1)[0];
                    GameObject o = Instantiate(imagePrefab, mapRoot);
                    o.name = tmpObjName.ToString();
                    o.layer = LayerMask.NameToLayer("Ground");
                    tmpObjName++;
                    o.GetComponent<SpriteRenderer>().sprite = sprite;
                    // o.GetComponent<Image>().SetNativeSize();
                    o.GetComponent<Transform>().localPosition = new Vector2(
                        (i - level.Width / 2.0f) * sprite.rect.width / 100,
                        -1 * (j - level.Height / 2.0f) * sprite.rect.height / 100);
                    level.LevelObj.Add(o);

                    switch (level.LevelMap[i, j])
                    {
                        case LevelItemType.Road:
                            DestroyImmediate(o.GetComponent<Collider2D>());
                            break;
                    }
                }
            }

            if (Application.isPlaying)
                LevelManager.Instance.isLevelLoaded = true;
        }

        [HorizontalGroup("Button")]
        [Button("清空场景", ButtonSizes.Large)]
        public void ClearChildren()
        {
            List<GameObject> tmpDesObjs = new List<GameObject>();
            for (int i = 0; i < mapRoot.childCount; i++)
            {
                tmpDesObjs.Add(mapRoot.GetChild(i).gameObject);
            }

            foreach (var t in tmpDesObjs)
            {
                DestroyImmediate(t);
            }
        }
    }
}