/*
** Created by fengling
** DateTime:    2021-05-13 11:23:16
** Description: TODO 
*/

using Framework.Scripts.UI.Base;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Framework.Scripts.UI.CustomUI
{
    public class CustomPanel : UiWidgetBase
    {
        [ShowInInspector] [ReadOnly]
        public CanvasGroup canvasGroup;
        public void ShowWithHiddenTurn()
        {
            canvasGroup = (CanvasGroup) Constants.Constants.AddOrGetComponent(gameObject, typeof(CanvasGroup));
            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha - 1);
            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }
    }
}