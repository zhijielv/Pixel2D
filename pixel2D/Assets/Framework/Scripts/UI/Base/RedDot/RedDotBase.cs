/*
** Created by fengling
** DateTime:    2021-04-27 10:02:12
** Description: TODO 红点组件逻辑
*/

using DG.Tweening;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Scripts.UI.Base.RedDot
{
    public partial class RedDotBase : UiWidgetBase
    {
        public Image redImage;

        // 添加红点监听
        public void AddRedDotEventListener(EventConstants type)
        {
            EventManager.Instance.AddEventListener(type, OnShow);
        }
        
        // 红点
        public void OnShow(EventData data)
        {
            ShowRedDot();
        }
        
        private void Start()
        {
            redImage = transform.GetComponent<Image>();
            AddInterFaceEvent(EventTriggerType.PointerClick, OnClick);
        }
        
        // 设置红点状态
        private void SetState(bool boolen)
        {
            redImage.DOColor(boolen ? Color.red : Color.blue, 0.1f);
        }

        private void OnClick(BaseEventData arg0)
        {
            UpdateRedDot();
        }
    }
}