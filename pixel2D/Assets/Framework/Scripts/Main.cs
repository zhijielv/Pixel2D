/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using System;
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
    public string widgetName;

    public AllViewEnum _AllViewList;
    public Main_View_Widget _MainViewWidget;
    private UiWidgetBase widgetObj;
    private void Awake()
    {
        gameObject.AddComponent<UiManager>();
        UiManager.Instance.mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        // Type t = Type.GetType(_AllViewList.ToString());
        //创建实例对象
        // var obj = t.Assembly.CreateInstance(_AllViewList.ToString());
        // widgetObj = UiManager.Instance.GetWidget<Main_View>();
    }
    
    [Button("Test Get Main_View 泛型")]
    public void TestGetMain_View()
    {
        // widgetObj = UiManager.Instance.GetWidget<Main_View>(_MainViewWidget);
        // widgetObj.TextWrite(widgetName);
        // Debug.Log(widgetObj.name);
    }
    
    [Button("Test Get Main_View Enum")]
    public void TestGetMain_View2()
    {
        widgetObj = UiManager.Instance.GetWidget(_AllViewList, _MainViewWidget);
        widgetObj.TextWrite(widgetName);
        // Debug.Log(widgetObj.name);
    }
    
    // [Button("Test Get Test_View")]
    public void TestGetTest_View()
    {
        // GameObject widgetObj = UiManager.Instance.GetWidget<Test_View>(widgetName);
        // Debug.Log(widgetObj.name);
    }

    [Button("Get Main_View Type")]
    public void GetTypeTest()
    {
        string t = "Main_View";
        Type type = Type.GetType(Framework.Constants.UiNameSpace + t);
        Debug.Log(Framework.Constants.UiNameSpace + t);
        if (type == null)
        {
            Debug.LogError($"{t} is not Generate");
        }
        else
        {
            Debug.LogWarning($"{t} is generate {type.FullName}");
        }
    }
}