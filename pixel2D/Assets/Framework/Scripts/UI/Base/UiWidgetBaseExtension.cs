/*
** Created by fengling
** DateTime:    2021-06-07 15:03:57
** Description: TODO 修改UiWidgetBase里的方法，逐步改为UniRx的static方法
*/

using System;
using UniRx;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Framework.Scripts.UI.Base
{
    public static class UiWidgetBaseExtension
    {
        public static IObservable<Unit> AddButtonClickEvent(this Button button, UnityAction callback)
        {
            IObservable<Unit> observable = button.onClick.AsObservable();
            observable.Subscribe(_=>callback());
            return observable;
        }
    }
}