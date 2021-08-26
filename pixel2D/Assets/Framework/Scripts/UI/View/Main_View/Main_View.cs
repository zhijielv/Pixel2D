//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System.Data;
using Framework.Scripts.Manager;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Framework.Scripts.Constants;
using Rewired;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Framework.Scripts.UI.View
{
    using Base;
    using System;
    using UnityEngine;

    public sealed partial class Main_View : ViewBase
    {
        public string FontGroupName = "Font";
        private void Start()
        {
            LeftTop_Text.text = "";
            RightTop_Text.text = "";
            BG_Image.SetNativeSize();
            float scale = BG_Image.rectTransform.sizeDelta.x /
                          Common.MainCanvas.GetComponent<CanvasScaler>().referenceResolution.x;
            BG_Image.rectTransform.sizeDelta /= scale;

            Setting_Button.AddButtonClickEvent(OnSettingBtnClick).Subscribe((unit =>
            {
                Debug.Log("on Button Click");
                Debug.Log(Language.Get(0));
            }));
            ChangeFont_Button.AddButtonClickEvent(ChangeFont);
            
            // Text widget = GetWidget("Center_Text") as Text;
            // widget.GetComponent<UiWidgetBase>().AddOnDragEvent("Center_Text", arg0 =>
            // {
            //     Vector2 mouseScreenPosition = RewiredInputEventManager.Instance.player0.controllers.Mouse.screenPosition;
            //
            //     Vector2 resultPos;
            //     RectTransformUtility.ScreenPointToLocalPointInRectangle(widget.transform.parent.GetComponent<RectTransform>(),
            //         mouseScreenPosition, Camera.main, out resultPos);
            //     widget.GetComponent<RectTransform>().position = resultPos;
            //     Debug.Log($"{widget.gameObject.GetComponent<RectTransform>().anchoredPosition} {resultPos}");
            // });
        }

        private void OnSettingBtnClick()
        {
            // SelectFont_CustomPanel.gameObject.SetActive(!SelectFont_CustomPanel.gameObject.activeSelf);
        }

        private void ChangeFont()
        {
            string text = Text_Dropdown.captionText.text;
            Text widget = (Text) GetWidget(text);
            widget.font = AddressableManager.Instance.LoadAsset<Font>(Font_Dropdown.captionText.text);
        }

        private void Update()
        {
            CenterTop_Text.text = System.DateTime.Now.ToString("yyyy-MM-dd dddd");
            Center_Text.text = System.DateTime.Now.ToString("HH:mm");
            CenterRight_Text.text = System.DateTime.Now.ToString("ss");
        }
    }
}
