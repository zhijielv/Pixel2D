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
using System;
using DG.Tweening;
using Sirenix.Utilities;
using UniRx;

namespace Framework.Scripts.Manager
{
    public class FrameWorkDebugMode : ManagerSingleton<FrameWorkDebugMode>
    {
        public GameObject runtimeHierarchy;
        public GameObject runtimeInspector;

        public override Task ManagerInit()
        {
            if (GlobalConfig<Common>.Instance.isDebugMode)
                Logging.Log("#####################  Is Debug Mode  #####################");
            else return base.ManagerInit();
            runtimeHierarchy = AddressableManager.Instance.Instantiate("RuntimeHierarchy");
            runtimeHierarchy.SetActive(false);
            runtimeInspector = AddressableManager.Instance.Instantiate("RuntimeInspector");
            runtimeInspector.SetActive(false);

            var observable = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.H));
            
            observable.Buffer(observable.Throttle(TimeSpan.FromMilliseconds(250)))
                .Where(xs => xs.Count >= 2)
                .Subscribe(xs =>
                {
                    ShowOrHideObj(runtimeHierarchy);
                });
            
            var observable2 = Observable.EveryUpdate()
                .Where(_ => Input.GetKeyDown(KeyCode.I));
            
            observable2.Buffer(observable2.Throttle(TimeSpan.FromMilliseconds(250)))
                .Where(xs => xs.Count >= 2)
                .Subscribe(xs =>
                {
                    ShowOrHideObj(runtimeInspector);
                });

            return base.ManagerInit();
        }

        public void ShowOrHideObj(GameObject obj)
        {
            if (obj.transform.parent == null)
            {
                obj.transform.SetParent(Common.MainCanvas.transform);
                obj.transform.DOScale(1, 0);
                
            }
            obj.SetActive(!obj.activeSelf);
        }
    }
}