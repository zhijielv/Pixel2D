using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Scripts.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject imagePrefab;
        public Transform mapRoot;
        [ShowInInspector] public Level level;

        private void Awake()
        {
            level = LevelManager.Instance.GetLevel();
        }

#if UNITY_EDITOR
        [OnValueChanged("LoadLevel")] public LevelType levelType;

        public void LoadLevel(LevelType levelType)
        {
            level = new Level();
            LeveljsonClass tmpLeveljsonClass = new LeveljsonClass();
            var list = JsonHelper.JsonReader<List<LeveljsonClass>>(Constants.Constants.JsonPath);
            foreach (var leveljsonClass in list)
            {
                if (!leveljsonClass.LevelType.Equals(levelType.ToString())) continue;
                tmpLeveljsonClass = leveljsonClass;
            }

            level.GenerateLevelValueFromJson(tmpLeveljsonClass);

            level.SetLevelMap();
        }

        [OnInspectorInit]
        public void Init()
        {
            LoadLevel(levelType);
        }

#endif

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
                    o.GetComponent<Image>().sprite = sprite;
                    o.GetComponent<Image>().SetNativeSize();
                    o.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        (i - level.Width / 2) * sprite.rect.width,
                        -1 * (j - level.Height / 2) * sprite.rect.height);
                    level.LevelObj.Add(o);

                    switch (level.LevelMap[i, j])
                    {
                        case LevelItemType.Road:
                            DestroyImmediate(o.GetComponent<Collider2D>());
                            break;
                    }
                }
            }
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