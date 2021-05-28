/*
** Created by fengling
** DateTime:    2021-05-10 10:50:33
** Description: 关卡地图数据
*/

using System.Collections.Generic;

namespace Framework.Scripts.Level.LevelItem
{
    public enum LevelType
    {
        ludi = 0,
        yanjiang,
        xueshan,
    }

    public enum LevelItemType
    {
        // 墙体
        Wall = 0,
        // 陆地
        Road,
        // 盒子
        Box,
        // 障碍物
        Obstruction,
        Maxvalue,
    }

    public class LeveljsonClass
    {
        public LevelType LevelType;
        public List<string> RoadList = new List<string>();
        public List<string> WallList = new List<string>();
        public List<string> BoxList = new List<string>();
        public List<string> ObstructionList = new List<string>();
    }
}