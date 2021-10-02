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
        [ShowInInspector] public Level Level;

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
            JsonHelper.JsonReader(out _levelMaps, Constants.Constants.MapJson);
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
            Level = new Level();
            LevelMapJsonClass levelMapJsonClass = levelMap[levelIndex];
            Level.LevelName = levelMapJsonClass.LevelName;
            Level.LevelType = levelMapJsonClass.LevelType;
            Level.SetSize(levelMapJsonClass.Width, levelMapJsonClass.Height);

            // 读取level
            levelName = Level.LevelName;
            levelType = Level.LevelType;
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            JsonHelper.JsonReader(out List<LeveljsonClass> tmpLeveljsonClass2, Constants.Constants.LevelJson);
            foreach (var leveljsonClass in tmpLeveljsonClass2)
            {
                if (leveljsonClass.LevelType != levelType) continue;
                tmpLeveljsonClass = leveljsonClass;
            }
            
            Level.GenerateLevelValueFromJson(tmpLeveljsonClass);

            Level.SetLevelWall();
            levelMapJsonClass.SetLevelRoad(Level);
            ClearChildren();
            CreateLevel();
        }
        
#endif
        // 创建关卡到游戏中
        [HorizontalGroup("Button")]
        [Button("创建关卡", ButtonSizes.Large)]
        public void CreateLevel()
        {
            Level.ClearLevelObj();
            // level.ChangeSize(0);
            int tmpObjName = 0;
            for (int i = 0; i < Level.LevelMap.GetLength(0); i++)
            {
                for (int j = 0; j < Level.LevelMap.GetLength(1); j++)
                {
                    List<Sprite> sprites = Level.LevelItem[Level.LevelMap[i, j]] ?? Level.LevelItem[LevelItemType.Road];
                    Sprite sprite = Constants.RandomHelper.GetRandomValueFromList(sprites, 1)[0];
                    GameObject o = Instantiate(imagePrefab, mapRoot);
                    o.name = tmpObjName.ToString();
                    tmpObjName++;
                    o.GetComponent<SpriteRenderer>().sprite = sprite;
                    // o.GetComponent<Image>().SetNativeSize();
                    o.GetComponent<Transform>().localPosition = new Vector2(
                        (i - Level.Width / 2.0f) * sprite.rect.width / 100,
                        -1 * (j - Level.Height / 2.0f) * sprite.rect.height / 100);
                    Level.LevelObj.Add(o);

                    switch (Level.LevelMap[i, j])
                    {
                        case LevelItemType.Road:
                            DestroyImmediate(o.GetComponent<Collider2D>()); break;
                        case LevelItemType.Box :
                            o.layer = LayerMask.NameToLayer("Box"); break;
                        case LevelItemType.Wall :
                            o.layer = LayerMask.NameToLayer("Wall"); break;
                        case LevelItemType.Obstruction:
                            break;
                        case LevelItemType.Maxvalue:
                            break;
                        default:
                            o.layer = LayerMask.NameToLayer("Ground"); break;
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