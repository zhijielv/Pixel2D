/*
** Created by fengling
** DateTime:    2021-06-15 16:30:59
** Description: TODO 
*/

using System;
using System.Threading.Tasks;
using DG.Tweening;
using Framework.Scripts.Constants;
using Framework.Scripts.Singleton;
using UniRx;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class DebugManager : ManagerSingleton<DebugManager>
    {
        public GameObject runtimeHierarchy;
        public GameObject runtimeInspector;

        public override Task Init()
        {
            if (!Common.IsDebugMode) return base.Init();
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

            return base.Init();
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