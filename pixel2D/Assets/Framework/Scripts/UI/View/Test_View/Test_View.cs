using Framework.Scripts.Manager;
using UnityEngine.Serialization;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;
    using Base.RedDot;
    using Constants;
    
    public sealed partial class TestView : ViewBase
    {
        [FormerlySerializedAs("RedDot")] public RedDotBase redDot;
        [FormerlySerializedAs("RedDot2")] public RedDotBase redDot2;
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                EventManager.Instance.DispatchEvent(Scripts.Constants.EventType.RedDot);
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                EventManager.Instance.DispatchEvent(Scripts.Constants.EventType.StartGame);
            }
        }

        private void Start()
        {
            // LevelManager.Instance.levelType = LevelType.yanjiang;
            // await LevelManager.Instance.LoadLevel(transform);
            redDot.AddRedDotEventListener(Scripts.Constants.EventType.RedDot);
            redDot2.AddRedDotEventListener(Scripts.Constants.EventType.StartGame);
        }
    }
}
