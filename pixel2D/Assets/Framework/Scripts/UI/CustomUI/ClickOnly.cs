/*
** Created by fengling
** DateTime:    2021-06-23 10:03:07
** Description: 接受点击事件的空ui 
*/

using UnityEngine;
using UnityEngine.EventSystems;

namespace Framework.Scripts.UI.CustomUI
{
    public class ClickOnly : CustomGraphic, IPointerClickHandler
    {
        
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(eventData.position + "  " + eventData.clickCount);
        }
    }
}