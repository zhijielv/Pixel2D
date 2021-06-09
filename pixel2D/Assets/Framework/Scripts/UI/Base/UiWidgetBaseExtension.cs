/*
** Created by fengling
** DateTime:    2021-06-07 15:03:57
** Description: TODO 修改UiWidgetBase里的方法，逐步改为UniRx的static方法
*/

using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.Scripts.UI.Base
{
    public static class UiWidgetBaseExtension
    {
        public static IObservable<Unit> AddButtonClickEvent(this Button button, UnityAction callback)
        {
            IObservable<Unit> observable = button.onClick.AsObservable();
            observable.Subscribe(_ => callback());
            return observable;
        }

        #region InputField
        public static IObservable<string> AddValueChangedEvent(this InputField inputField, UnityAction<string> callback)
        {
            IObservable<string> observable = inputField.OnValueChangedAsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        
        public static IObservable<string> AddEndEditEvent(this InputField inputField, UnityAction<string> callback)
        {
            IObservable<string> observable = inputField.onEndEdit.AsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        #endregion
        
        
        #region Toggle
        public static IObservable<bool> AddValueChangedEvent(this Toggle toggle, UnityAction<bool> callback)
        {
            IObservable<bool> observable = toggle.OnValueChangedAsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        #endregion
        
        #region Slider
        public static IObservable<float> AddValueChangedEvent(this Slider slider, UnityAction<float> callback)
        {
            IObservable<float> observable = slider.OnValueChangedAsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        #endregion
        
        #region Dropdown
        public static IObservable<int> AddValueChangedEvent(this Dropdown dropdown, UnityAction<int> callback)
        {
            IObservable<int> observable = dropdown.OnValueChangedAsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        #endregion
        
        #region ScrollRect
        public static IObservable<Vector2> AddValueChangedEvent(this ScrollRect scrollRect, UnityAction<Vector2> callback)
        {
            IObservable<Vector2> observable = scrollRect.OnValueChangedAsObservable();
            observable.Subscribe((s => callback(s)));
            return observable;
        }
        #endregion
        
        #region Text
        public static void TextWrite(this Text text, string value)
        {
            text.text = value;
        }

        public static void TextClear(this Text text)
        {
            text.text = null;
        }
        
        #endregion
    }
}