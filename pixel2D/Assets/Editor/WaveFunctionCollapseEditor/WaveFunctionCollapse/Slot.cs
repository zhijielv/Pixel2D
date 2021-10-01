/*
** Created by fengling
** DateTime:    2021-06-25 10:58:25
** Description: TODO 
*/

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Framework._3rdParty.WaveFunctionCollapse
{
    public class Slot
    {
        public static Vector2 SlotSize = new Vector2(0.32f, 0.32f);
        public Vector2Int Position;
        public Map Map;

        [ShowInInspector] public Module Module;

        // public List<Module> Modules;
        public List<Texture> Textures;

        // public Texture Texture;
        public Slot(Vector2Int position, Map map)
        {
            Position = position;
            Map = map;
            Textures = AssetDatabase.FindAssets("t:Texture", new[] {"Assets/Art/Sprite/Level/test"})
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(guid))).ToList();
        }

        public Texture GetRandom()
        {
            if (Textures.Count == 0) return null;
            int range = Random.Range(0, Textures.Count);
            Debug.Log(Position.ToString() + "   " + Textures.Count + "  " + range);
            Texture texture = Textures[range];

            foreach (ModuleData data in Map.ModuleDatas)
            {
                if (data.Module.Equals(texture.name))
                {
                    Module = new Module(data.Module, texture, data.PossibleNeighborsArray);
                    // todo 传播
                    Fits(texture.name);
                }
            }

            Textures.Clear();
            Textures.Add(texture);

            return texture;
        }

        public void Fits(string textureName)
        {
            // 上
            if (Position.y - 1 >= 0)
            {
                Map.Slots[Position.x, Position.y - 1].Remove(0, textureName);
            }

            // 左
            if (Position.x - 1 >= 0)
            {
                Map.Slots[Position.x - 1, Position.y].Remove(1, textureName);
            }

            // 下
            if (Position.y + 1 < Map.Slots.GetLength(1))
            {
                Map.Slots[Position.x, Position.y + 1].Remove(2, textureName);
            }

            // 右
            if (Position.x + 1 < Map.Slots.GetLength(0))
            {
                Map.Slots[Position.x + 1, Position.y].Remove(3, textureName);
            }
        }

        public void Remove(int direction, string textureName)
        {
            if(Map.Bools[Position.x, Position.y]) return;
            string[] neighbors = null;
            foreach (ModuleData moduleData in Map.ModuleDatas)
            {
                if (moduleData.Module.Equals(textureName))
                {
                    neighbors = moduleData.PossibleNeighborsArray[direction];
                    break;
                }
            }

            List<Texture> tmpTextures = new List<Texture>();
            
            foreach (Texture texture in Textures)
            {
                if(neighbors == null) break;
                if (neighbors.Contains(texture.name))
                {
                    tmpTextures.Add(texture);
                }
            }
            
            // todo 往下为空，回溯

            Textures = tmpTextures;
        }

        public void Reset()
        {
            // 上
            if (Position.y - 1 >= 0 && !Map.Bools[Position.x, Position.y - 1])
            {
                Map.Slots[Position.x, Position.y - 1] = new Slot(new Vector2Int(Position.x, Position.y - 1), Map);
            }

            // 左
            if (Position.x - 1 >= 0 && !Map.Bools[Position.x - 1, Position.y])
            {
                Map.Slots[Position.x - 1, Position.y] = new Slot(new Vector2Int(Position.x - 1, Position.y), Map);
            }

            // 下
            if (Position.y + 1 < Map.Slots.GetLength(1) && !Map.Bools[Position.x, Position.y + 1])
            {
                Map.Slots[Position.x, Position.y + 1] = new Slot(new Vector2Int(Position.x, Position.y + 1), Map);
            }

            // 右
            if (Position.x + 1 < Map.Slots.GetLength(0) && !Map.Bools[Position.x + 1, Position.y])
            {
                Map.Slots[Position.x + 1, Position.y] = new Slot(new Vector2Int(Position.x + 1, Position.y - 1), Map);
            }
        }
    }
}