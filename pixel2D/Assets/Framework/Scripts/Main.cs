/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using System;
using System.Globalization;
using System.Reflection;
using Framework.Scripts.UI.Base;
using Framework.Scripts.UI.View;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    private Text leftTopText;
    private Text centerText;
    private UiWidgetBase centerRightText;
    private void Awake()
    {
        gameObject.AddComponent<UiManager>();
        UiManager.Instance.mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        leftTopText = (Text) UiManager.Instance.GetWidget<Main_View>(Main_View_Widget.LeftTop_Text);
        centerText = UiManager.Instance.GetWidget(AllViewEnum.Main_View, Main_View_Widget.Center_Text).GetComponent<Text>();
        centerRightText = UiManager.Instance.GetWidget(AllViewEnum.Main_View, Main_View_Widget.CenterRight_Text);
        UiManager.Instance.GetWidgetComponent<Text>(AllViewEnum.Main_View, Main_View_Widget.CenterTop_Text).text = "";
        Text rightTopText = (Text) UiManager.Instance.GetWidgetObj<Main_View>(Main_View_Widget.RightTop_Text);
        rightTopText.text = "";
    }

    private void Update()
    {
        leftTopText.text = System.DateTime.Now.ToString("yyyy-MM-dd dddd");
        centerText.text = System.DateTime.Now.ToString("HH:mm");
        centerRightText.TextWrite(System.DateTime.Now.ToString("ss"));
    }
}