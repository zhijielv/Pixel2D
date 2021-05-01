/*
** Created by fengling
** DateTime:    2021年5月1日 20:19:31
** Description: UI界面基类
*/

using UnityEngine;

namespace Framework.Scripts.UI.CustomUI
{
    public class QuitGame : MonoBehaviour
    {
        public void QuitThisGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}