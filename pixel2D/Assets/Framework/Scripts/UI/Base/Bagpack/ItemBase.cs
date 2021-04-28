/*
** Created by fengling
** DateTime:    2021-04-27 15:12:01
** Description: TODO 
*/

using System.Collections.Generic;
using Framework.Scripts.Constants;
using Framework.Scripts.Manager;
using UnityEngine;

namespace Framework.Scripts.UI.Base.Bagpack
{
    public class ItemBase : UiWidgetBase
    {
        public string itemName;
        public uint num = 0;

        private void Start()
        {
            EventManager.Instance.AddEventListener(EventConstants.ItemChangeNum, OnValueChanged);
        }

        private void OnValueChanged(EventData data)
        {
            num = uint.Parse( Mathf.Clamp((int)data.Data, 0, 99).ToString());
            List<ItemBase> list = new List<ItemBase>();
            list.Add(this);
            EventManager.Instance.DispatchEvent(EventConstants.BagpackChange, list);
        }
    }
}