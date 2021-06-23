/*
** Created by fengling
** DateTime:    2021-06-23 09:54:01
** Description: 接受Raycast的空UI组件 
*/

using UnityEngine;
using UnityEngine.UI;

namespace Framework.Scripts.UI.CustomUI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public abstract class CustomGraphic : Graphic
    {
        protected override void Start()
        {
            base.Start();
            this.color = Color.clear;
        }
    }
}