/*
** Created by fengling
** DateTime:    2021-04-27 15:12:01
** Description: TODO 背包格子类
*/

using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using UnityEngine;
using Debug = System.Diagnostics.Debug;
using EventType = Framework.Scripts.Constants.EventType;

namespace Framework.Scripts.UI.Base.Bagpack
{
    public class ItemBase : UiWidgetBase
    {
        public string itemName;
        public uint num = 0;

        private void Start()
        {
            EventManager.Instance.AddEventListener(EventType.ItemChangeNum, OnValueChanged);
        }

        private void OnValueChanged(object sender, EventArgs e)
        {
            EventData<int> data = e as EventData<int>;
            Debug.Assert(data != null, nameof(data) + " != null");
            num = uint.Parse(Mathf.Clamp(data.Value, 0, 99).ToString());
            List<ItemBase> list = new List<ItemBase>();
            list.Add(this);
            EventManager.Instance.DispatchEvent(EventType.BagpackChange, new EventData(){Data = list});
        }
    }
}