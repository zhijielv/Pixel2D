/*
** Created by fengling
** DateTime:    2021-06-25 11:02:53
** Description: TODO 
*/

using System.Collections.Generic;
using Framework.Scripts.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Framework._3rdParty.WaveFunctionCollapse
{
    public class Map
    {
        private readonly Vector2Int _size;

        [ShowInInspector] [TableMatrix(DrawElementMethod = "DrawSlot", SquareCells = true)]
        public readonly Slot[,] Slots;

        public static bool[,] Bools;

        public List<ModuleData> ModuleDatas;

        public Map(Vector2Int size)
        {
            _size = size;
            Slots = new Slot[size.x, size.y];
            Bools = new bool[size.x, size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    Slots[i, j] = new Slot(new Vector2Int(i, j), this);
                    Bools[i, j] = false;
                }
            }
            
            JsonHelper.JsonReader(out ModuleDatas, "WFCModule");
        }

        static Slot DrawSlot(Rect rect, Slot[,] slots, int x, int y)
        {
            int index = 0;
            if (!Bools[x, y])
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Rect tmpRect =
                            new Rect(
                                new Vector2(rect.position.x + (i * rect.size.x / 3),
                                    rect.position.y + (j * rect.size.y / 3)),
                                rect.size / 3);

                        if (index < slots[x, y].Textures.Count)
                        {
                            SirenixEditorGUI.IconButton(tmpRect, slots[x, y].Textures[index++], GUIStyle.none, "");
                        }
                    }
                }
            }
            else
            {
                Rect tmpRect = new Rect(new Vector2(rect.position.x, rect.position.y), rect.size / 3);
                if(slots[x, y].Textures.Count > 0)
                    SirenixEditorGUI.IconButton(tmpRect, slots[x, y].Textures[0], GUIStyle.none, "");
            }

            if (rect.Contains(Event.current.mousePosition))
            {
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    Texture texture = slots[x, y].GetRandom();
                    Bools[x, y] = true;
                }
                else if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    slots[x, y] = new Slot(new Vector2Int(x, y), slots[x, y].Map);
                    slots[x, y].Reset();
                    Bools[x, y] = false;
                }
            }

            return slots[x, y];
        }
    }
}