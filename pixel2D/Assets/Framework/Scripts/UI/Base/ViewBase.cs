/*
** Created by fengling
** DateTime:    2021-03-21 14:09:20
** Description: UI界面基类
*/

using System;
using System.Reflection;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework.Scripts.UI.Base
{
    public class ViewBase : UiWidgetBase
    {
        [ShowInInspector] [ReadOnly] public CanvasGroup canvasGroup;

        #region private

        #endregion

        #region public
        
        internal virtual object GetWidget(string widgetName)
        {
            Type viewType = GetType();
            // 反射赋值
            FieldInfo[] fieldInfos = viewType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                if (fieldInfo.Name != widgetName) continue;
                var obj = fieldInfo.GetValue(gameObject.GetComponent<ViewBase>());
                var o = Convert.ChangeType(obj, fieldInfo.FieldType);
                return o;
            }

            return null;
        }

        public void ChangePanel<T>() where T : ViewBase
        {
            if (typeof(T) == GetType()) return;
            ShowWithHiddenTurn();
            ViewBase targetView = (ViewBase) UiManager.Instance.GetWidget<T>();
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
            canvasGroup = (CanvasGroup) Constants.AddOrGetComponent(gameObject, typeof(CanvasGroup));
            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha - 1);
            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }

        #endregion

        #region virtual

        #endregion
    }
}