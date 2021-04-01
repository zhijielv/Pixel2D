/*
** Created by fengling
** DateTime:    2021-03-21 14:09:20
** Description: UI界面基类
*/

using System;
using System.Collections.Generic;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Framework.Scripts.UI.Base
{
    public class ViewBase : UiWidgetBase
    {
        [ShowInInspector]
        // [ReadOnly]
        [OnValueChanged("Valuechange")]
        public List<GameObject> widgetDic;
        public CanvasGroup canvasGroup;

        public GameObject target;

        #region private
        #endregion

        public void Valuechange(List<GameObject> widgetDic)
        {
            Debug.Log(widgetDic.Count);
        }
        #region public
        public UiWidgetBase GetWidget(string widgetName)
        {
            foreach (GameObject o in widgetDic)
            {
                if (o.name.EndsWith(widgetName))
                    return o.GetComponent<UiWidgetBase>();
            }
            // if (widgetDic.ContainsKey(widgetName))
            // {
            //     return widgetDic[widgetName].GetComponent<UiWidgetBase>();
            // }
            Debug.LogError(gameObject.name + " has not widget : " + widgetName);
            return null;
        }
#if UNITY_EDITOR
        public void ResetData()
        {
            Debug.Log("reset Data");
            // target = transform.GetChild(0).gameObject;
            widgetDic = new List<GameObject>();
            foreach (UiWidgetBase uiWidgetBase in transform.GetComponentsInChildren<UiWidgetBase>())
            {
                widgetDic.Add(uiWidgetBase.gameObject);
            }

            canvasGroup = (CanvasGroup) Constants.AddOrGetComponent(gameObject, typeof(CanvasGroup));
        }  
#endif
        public void ChangePanel<T>() where T : ViewBase
        {
            ShowWithHiddenTurn();
            ViewBase targetView =  (ViewBase) UiManager.Instance.GetWidget<T>();
            targetView.ShowWithHiddenTurn();
        }

        public void AddButtonClickEvent(string widgetName, UnityAction callback)
        {
            AddButtonClickEvent(callback);
        }

        public void AddInputFieldValueChangedEvent(string widgetName, UnityAction<string> callback)
        {
            AddInputFieldValueChangedEvent(callback);
        }

        public void AddInputFieldEndEditEvent(string widgetName, UnityAction<string> callback)
        {
            AddInputFieldEndEditEvent(callback);
        }

        public void TextWrite(string widgetName, string value)
        {
            TextWrite(value);
        }

        public void TextClear(string widgetName)
        {
            TextClear();
        }

        public void AddToggleValueChangedEvent(string widgetName, UnityAction<bool> callback)
        { 
            AddToggleValueChangedEvent(callback);
        }

        public void AddSliderValueChangedEvent(string widgetName, UnityAction<float> callback)
        {
            AddSliderValueChangedEvent(callback);
        }

        public void AddDropdownValueChangedEvent(string widgetName, UnityAction<int> callback)
        {
            AddDropdownValueChangedEvent(callback);
        }

        public void AddScrollValueChangedEvent(string widgetName, UnityAction<Vector2> callback)
        {
            AddScrollValueChangedEvent(callback);
        }


        public void AddClickEvent(string widgetName, UnityAction<BaseEventData> callback)
        {
            AddInterFaceEvent(EventTriggerType.PointerClick, callback);
        }

        public void AddClickEvent(string subParentName, string subName, UnityAction<BaseEventData> callback)
        {
            AddInterFaceEvent(EventTriggerType.PointerClick, callback);
        }

        public void AddBeginDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.BeginDrag, callBack);
        }

        public void AddBeginDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.BeginDrag, callBack);
        }

        public void AddOnDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.Drag, callBack);
        }

        public void AddOnDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.Drag, callBack);
        }

        public void AddEndDragEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.EndDrag, callBack);
        }

        public void AddEndDragEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.EndDrag, callBack);
        }

        public void AddPointerEnterEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerEnter, callBack);
        }

        public void AddPointerEnterEvent(string subPaentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerEnter, callBack);
        }

        public void AddPointerExitEvent(string widgetName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerExit, callBack);
        }

        public void AddPointerExitEvent(string subParentName, string subName, UnityAction<BaseEventData> callBack)
        {
            AddInterFaceEvent(EventTriggerType.PointerExit, callBack);
        }

        public void ShowWithHiddenTurn()
        {
            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha - 1);
            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }

        #endregion

        #region virtual

        // todo 使用子类的Enum调用
        public virtual UiWidgetBase GetWidget(Enum @enum = null)
        {
            if (@enum != null) return GetWidget(@enum.ToString());
            return GetWidget(gameObject.name);
        }

        #endregion
    }
}