/*
** Created by fengling
** DateTime:    2021-03-21 14:05:16
** Description: 所有UI操作的基类
** TODO : 参数改为EventHandler
*/

using Framework.Scripts.Manager;
using Sirenix.OdinInspector;
using SRF;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Framework.Scripts.UI.Base
{
    public class UiWidgetBase : MonoBehaviour
    {
        [ShowInInspector] [ReadOnly] public CanvasGroup canvasGroup;

        public void AddInterFaceEvent(EventTriggerType eventTriggerType, UnityAction<BaseEventData> callback)
        {
            EventTrigger tmpTrigger = GetComponent<EventTrigger>();
            if (tmpTrigger == null)
            {
                tmpTrigger = gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry tmpEntry = new EventTrigger.Entry();
            tmpEntry.eventID = eventTriggerType;
            tmpEntry.callback = new EventTrigger.TriggerEvent();
            tmpEntry.callback.AddListener(callback);
            tmpTrigger.triggers.Add(tmpEntry);
        }

        
        public void AddClickEvent(string widgetName, UnityAction<BaseEventData> callback)
        {
            AddInterFaceEvent(EventTriggerType.PointerClick, callback);
        }

        public void AddBeginDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.BeginDrag, callBack);
        }

        public void AddOnDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.Drag, callBack);
        }

        public void AddEndDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.EndDrag, callBack);
        }

        public void AddPointerEnterEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerEnter, callBack);
        }

        public void AddPointerExitEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerExit, callBack);
        }
        
        public void ShowOrHiddenTurn()
        {
            canvasGroup = gameObject.GetComponentOrAdd<CanvasGroup>();
            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha - 1);
            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }
    }
}