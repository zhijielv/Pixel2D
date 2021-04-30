using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Manager;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Scripts.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject imagePrefab;
        public Transform mapRoot;
        [ShowInInspector] public Level level;

        public async Task Init()
        {
            level = await LevelManager.Instance.GetLevel();
            // imagePrefab =
            // await AddressableManager.Instance.LoadAsset<GameObject>(Constants.Constants.LevelPrefabDir +
            // "Wall/f601/wall.prefab");
            imagePrefab = await ObjectManager.Instance.LoadUnit();
            mapRoot = transform;
            ResetTransform();
        }

        private void ResetTransform()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = new Quaternion(0, 0, 0, 1);
            transform.localScale = Vector3.one;
        }

        [OnValueChanged("LoadLevel")] public LevelType levelType;

        // 加载Level数据
        public async Task LoadLevel(LevelType levelType)
        {
            level = new Level();
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            var list = await JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.MapJson);
            foreach (var leveljsonClass in list)
            {
                if (!leveljsonClass.LevelType.Equals(levelType.ToString())) continue;
                tmpLeveljsonClass = leveljsonClass;
            }

            await level.GenerateLevelValueFromJson(tmpLeveljsonClass);

            level.SetLevelMap();
        }

        // 仅Inspector显示用
        [OnInspectorInit]
        public async void InspectorInit()
        {
            await LoadLevel(levelType);
        }

        // 创建关卡到游戏中
        [HorizontalGroup("Button")]
        [Button("创建关卡", ButtonSizes.Large)]
        public void CreateLevel()
        {
            level.ClearLevelObj();
            level.ChangeSize(0);
            int tmpObjName = 0;
            for (int i = 0; i < level.LevelMap.GetLength(0); i++)
            {
                for (int j = 0; j < level.LevelMap.GetLength(1); j++)
                {
                    List<Sprite> sprites = level.LevelItem[level.LevelMap[i, j]] ?? level.LevelItem[LevelItemType.Road];
                    Sprite sprite = Constants.RandomHelper.GetRandomValueFromList(sprites, 1)[0];
                    GameObject o = Instantiate(imagePrefab, mapRoot);
                    o.name = tmpObjName.ToString();
                    tmpObjName++;
                    o.GetComponent<SpriteRenderer>().sprite = sprite;
                    // o.GetComponent<Image>().SetNativeSize();
                    o.GetComponent<Transform>().localPosition = new Vector2(
                        (i - level.Width / 2) * sprite.rect.width / 100,
                        -1 * (j - level.Height / 2) * sprite.rect.height / 100);
                    level.LevelObj.Add(o);

                    switch (level.LevelMap[i, j])
                    {
                        case LevelItemType.Road:
                            DestroyImmediate(o.GetComponent<Collider2D>());
                            break;
                    }
                }
            }

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