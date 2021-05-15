/*
** Created by fengling
** DateTime:    2021-05-14 16:47:47
** Description: TODO Debug模式下，各种检测工具加载
 * SRDebuger
 * RuntimeRuntimeInspector
 * Graphy
*/

using System.Threading.Tasks;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class FrameWorkDebugMode : ManagerSingleton<FrameWorkDebugMode>
    {
        public override Task Init()
        {
            if (Common.IsDebugMode)
                Debug.Log("#####################  Is Debug Mode  #####################");
            return base.Init();
        }
    }
}