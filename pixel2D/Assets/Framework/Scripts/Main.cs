/*
** Created by fengling
** DateTime:    2021-03-21 11:36:57
** Description: 
*/

using Framework.Scripts.UI.View;
using Manager;
using Sirenix.OdinInspector;
using UnityEngine;

public class Main : MonoBehaviour
{
    [Tooltip("测试代码")]
    public string widgetName;
    private void Awake()
    {
        gameObject.AddComponent<UiManager>();
        UiManager.Instance.mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        
    }
    
    [Button("Test Get Main_View")]
    public void TestGetMain_View()
    {
        GameObject widgetObj = UiManager.Instance.GetView<Main_View>(Main_View_Enum.Bottom_Panel);
        // Debug.Log(widgetObj.name);
    }
    
    [Button("Test Get Test_View")]
    public void TestGetTest_View()
    {
        // GameObject widgetObj = UiManager.Instance.GetView<Test_View>(widgetName);
        // Debug.Log(widgetObj.name);
    }
}