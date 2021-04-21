using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.Scripts.Level
{
    public class LevelLoader : MonoBehaviour
    {
        public GameObject ImagePrefab;

        public GameObject MapRoot;

        [ShowInInspector] public Level level = new Level();

        [Button(ButtonSizes.Large)]
        public void LoadLevel()
        {
            level.SetSize();
            SetBorder(level);
        }

        public void SetBorder(Level level)
        {
            for (int i = 0; i < level.LevelMap.GetLength(0); i++)
            {
                level.LevelMap[i, 0] = 1;
                level.LevelMap[i, level.Height - 1] = 1;
            }

            for (int i = 0; i < level.LevelMap.GetLength(1); i++)
            {
                level.LevelMap[0, i] = 1;
                level.LevelMap[level.Width - 1, i] = 1;
            }
        }

        [Button("testRandom")]
        public void testGetRandom()
        {
            List<Sprite> sprites = level.LevelItem[(LevelItemType) level.LevelMap[0, 0]];
            List<Sprite> list = Constants.Constants.GetRandomValueFromList(sprites, 1);
            foreach (Sprite sprite in list)
            {
                Debug.Log($"{sprite.name}");
            }
        }

        public void CreateLevel(Level level)
        {
            int name = 0;
            for (int i = 0; i < level.LevelMap.GetLength(0); i++)
            {
                for (int j = 0; j < level.LevelMap.GetLength(1); j++)
                {
                    List<Sprite> sprites = level.LevelItem[(LevelItemType) level.LevelMap[i, j]];
                    Sprite sprite = Constants.Constants.GetRandomValueFromList(sprites, 1)[0];
                    GameObject o = Instantiate(ImagePrefab, transform);
                    o.name = name.ToString();
                    name++;
                    o.GetComponent<Image>().sprite = sprite;
                    o.GetComponent<Image>().SetNativeSize();
                    o.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                        (i - level.Width) * sprite.rect.width,
                        (j - level.Height) * sprite.rect.height);
                    level.LevelObj.Add(o);

                    switch (level.LevelMap[i, j])
                    {
                        case (int) LevelItemType.Door:
                            o.GetComponent<Collider2D>().isTrigger = true;
                            break;
                        case (int) LevelItemType.Road:
                            DestroyImmediate(o.GetComponent<Collider2D>());
                            break;
                    }
                }
            }
        }


        [Button]
        public void ClearChildren()
        {
            List<GameObject> tmpDesObjs = new List<GameObject>();
            for (int i = 0; i < transform.childCount; i++)
            {
                tmpDesObjs.Add(transform.GetChild(i).gameObject);
            }

            foreach (var t in tmpDesObjs)
            {
                DestroyImmediate(t);
            }
        }
    }
}