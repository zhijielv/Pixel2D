using Framework.Scripts.Manager;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    using Base.RedDot;
    using Constants;
    
    public partial class Test_View : ViewBase
    {
        public RedDotBase RedDot;
        public RedDotBase RedDot2;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                EventManager.Instance.DispatchEvent(EventConstants.RedDot);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventManager.Instance.DispatchEvent(EventConstants.StartGame);
            }
        }

        private void Start()
        {
            // LevelManager.Instance.levelType = LevelType.yanjiang;
            // await LevelManager.Instance.LoadLevel(transform);
            RedDot.AddRedDotEventListener(EventConstants.RedDot);
            RedDot2.AddRedDotEventListener(EventConstants.StartGame);
        }
    }
}
