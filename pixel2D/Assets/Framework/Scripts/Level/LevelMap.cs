/*
** Created by fengling
** DateTime:    2021-05-10 11:00:01
** Description: 总地图，保存关卡list
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Level.LevelItem;
using Framework.Scripts.Manager;
using UnityEngine;

namespace Framework.Scripts.Level
{
    public class LevelMap
    {
        public List<LevelMapJsonClass> Levels;

        public LevelMap()
        {
            Levels = new List<LevelMapJsonClass>();
        }

        public LevelMapJsonClass this[int index] => Get(index);

        public void Add(LevelMapJsonClass levelItem)
        {
            Levels.Add(levelItem);
        }

        public Level Generate2Level(int index)
        {
            LevelMapJsonClass levelMapJsonClass = this[index];
            Level level = new Level();
            level = level.GetLevel(levelMapJsonClass.LevelType);
            // Level level = await LevelManager.Instance.GetLevel(levelMapJsonClass.LevelType);
            level.LevelName = levelMapJsonClass.LevelName;
            level.LevelType = levelMapJsonClass.LevelType;
            level.Width = levelMapJsonClass.Width;
            level.Height = levelMapJsonClass.Height;
            level.LevelMap = new LevelItemType[level.Width, level.Height];
            levelMapJsonClass.SetLevelRoad(level);
            return level;
        }

        public LevelMapJsonClass Get(int index)
        {
            return Levels[index];
        }
    }
    
    
    public class LevelMapJsonClass
    {
        public string LevelName;
        public LevelType LevelType;
        public int Width;
        public int Height;
        public Dictionary<LevelItemType, int> WidgetDictionary;
        
        public Level this[int index] => Get(index);

        private Level Get(int index)
        {
            // Level level = await LevelManager.Instance.GetLevel(LevelType);
            Level level = new Level();
            level = level.GetLevel(LevelType);
            level.LevelName = LevelName;
            level.LevelType = LevelType;
            level.Width = Width;
            level.Height = Height;
            level.LevelMap = new LevelItemType[level.Width, level.Height];
            SetLevelRoad(level);
            return level;
        }


        public void SetLevelRoad(Level level)
        {
            if(WidgetDictionary.Count == 0) return;
            for (int i = 1; i < Width - 1; i++)
            {
                for (int j = 1; j < Height - 1; j++)
                {
                    level.LevelMap[i, j] = RandomHelper.GetWidgetRandom(WidgetDictionary);
                }
            }
        }
    }
}