/*
** Created by fengling
** DateTime:    2021-04-27 10:06:40
** Description: TODO 红点逻辑类
*/

using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Framework.Scripts.UI.Base.RedDot
{
    public partial class RedDotBase
    {
        public int redNum = 0;
        public bool isShow = false;
        public RedDotBase parentRedDot = null;
        [ShowInInspector] public List<RedDotBase> childrenRedDots = new List<RedDotBase>();

        private void SetRedDot()
        {
            // 叶子节点 设为1
            if (childrenRedDots.Count == 0)
                redNum = 1;
            else
                // 非叶子节点记录下一层总数
                RefreshRadDotNum();
            SetState(true);
            if (parentRedDot && !isShow)
                parentRedDot.SetRedDot();
            isShow = true;
        }

        private void UpdateRedDot()
        {
            RefreshRadDotNum();
            // 更新父节点
            if (parentRedDot)
                parentRedDot.UpdateRedDot();
            isShow = redNum != 0;
            SetState(redNum != 0);
        }

        private void RefreshRadDotNum()
        {
            redNum = 0;
            foreach (RedDotBase redDot in childrenRedDots)
            {
                redNum += redDot.redNum;
            }
        }
    }
}