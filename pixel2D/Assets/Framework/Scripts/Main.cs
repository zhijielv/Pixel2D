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
    [Tooltip("测试代码")]
    public string text = "测试代码";

    public AllViewEnum _AllViewList;
    public Main_View_Widget _MainViewWidget;
    private UiWidgetBase widgetObj;
    private Text MainViewCenterText;
    private void Awake()
    {
        gameObject.AddComponent<UiManager>();
        UiManager.Instance.mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        MainViewCenterText = UiManager.Instance.GetWidgetComponent<Text>(AllViewEnum.Main_View, Main_View_Widget.Center_Text);
        MainViewCenterText.text = DateTime.Now.ToShortTimeString();
    }

    [Button("任意类型")]
    public void TestGetMain_View()
    {
        UiManager.Instance.GetWidgetComponent<Text>(_AllViewList, _MainViewWidget).text = text;
        
        object component = UiManager.Instance.GetWidgetComponent(_AllViewList, _MainViewWidget);
        Debug.Log($"获取到：{component}         类型为：{component.GetType()}");
    }
    
    [Button("Enum 获取 widget")]
    public void TestGetMain_View2()
    {
        widgetObj = UiManager.Instance.GetWidget(_AllViewList, _MainViewWidget);
        Debug.Log(widgetObj.name);
        widgetObj.GetComponent<UiWidgetBase>().TextWrite(text);
        // Debug.Log(widgetObj.name);
    }
}