/*
** Created by fengling
** DateTime:    2021-05-25 17:32:06
** Description: TODO 编辑器辅助工具
*/

using System;
using Framework.Scripts.PlayerControl;
using Sirenix.Utilities;
using UnityEngine;

namespace Framework.Scripts.Utils
{
    [GlobalConfig("Editor/LevelEditor")]
    public class LevelHelper : GlobalConfig<LevelHelper>
    {
        /*private const string _assetPath = "Assets/Plugins/Sirenix/Odin Inspector/Config/Resources/Sirenix/LevelHelper.asset";

        public void CreatThisInstance()
        {
            AssetDatabase.CreateAsset(CreateInstance<LevelHelper>(), _assetPath);
            AssetDatabase.Refresh();
        }*/
        
        // todo 编辑器下控制相关物体
        public GameObject mainPlayer;
        public GameWeaponController weaponController;

        private void OnDisable()
        {
            mainPlayer = null;
            weaponController = null;
        }
    }
}