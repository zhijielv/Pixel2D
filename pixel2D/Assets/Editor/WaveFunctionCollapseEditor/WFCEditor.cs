/*
** Created by fengling
** DateTime:    2021-06-25 11:07:43
** Description: TODO 
*/

using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using Framework._3rdParty.WaveFunctionCollapse;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Editor.WaveFunctionCollapseEditor
{
    public class WFCEditor
    {
        [OnValueChanged("ChangeMapSize")]
        public Vector2Int MapSize = new Vector2Int(5, 5);
        [ShowInInspector]
        public Map Map;

        public GameObject MapRoot;

        public WFCEditor()
        {
            ChangeMapSize(MapSize);
            MapRoot = GameObject.Find("MapRoot");
            if (MapRoot == null)
            {
                MapRoot = new GameObject("MapRoot");
            }
        }

        public void ChangeMapSize(Vector2Int mapSize)
        {
            Map = new Map(mapSize);
        }
        
        [Button]
        public void CreateWFCMap()
        {
            Map = new Map(MapSize);
            for (int i = 0; i < MapSize.x; i++)
            {
                for (int j = 0; j < MapSize.y; j++)
                {
                    Texture texture = Map.Slots[i, j].GetRandom();
                    Map.Bools[i, j] = true;
                }
            }
        }

        [ButtonGroup("Map")]
        [Button("创建地图")]
        public void InstantiateMap()
        {
            if (MapRoot != null)
            {
                GameObject.DestroyImmediate(MapRoot);
                MapRoot = new GameObject("MapRoot");
            }
            foreach (Slot slot in Map.Slots)
            {
                if(slot.Textures.Count == 0) continue;
                Texture t2d = slot.Textures[0];
                Sprite s = Sprite.Create(t2d as Texture2D, new Rect(0, 0, t2d.width, t2d.height), Vector2.zero);
                GameObject obj = new GameObject {name = t2d.name};
                obj.transform.SetParent(MapRoot.transform);
                SpriteRenderer spriteRenderer = obj.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = s;
                obj.transform.localPosition = new Vector3(slot.Position.x * Slot.SlotSize.x, slot.Position.y * Slot.SlotSize.y);
            }
        }
    }
}