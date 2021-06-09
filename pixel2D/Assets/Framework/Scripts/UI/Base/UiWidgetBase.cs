﻿/*
** Created by fengling
** DateTime:    2021-03-21 14:05:16
** Description: 所有UI操作的基类
** TODO : 参数改为EventHandler
*/

using Sirenix.OdinInspector;
using SRF;
using UnityEngine;
using UnityEngine.UI;
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


        public void ShowWithHiddenTurn()
        {
            canvasGroup = gameObject.GetComponentOrAdd<CanvasGroup>();
            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha - 1);
            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }
    }
}