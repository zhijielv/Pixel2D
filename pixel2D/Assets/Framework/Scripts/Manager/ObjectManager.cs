﻿/*
** Created by fengling
** DateTime:    2021-04-28 17:10:19
** Description: TODO 对象池
*/

using System;
using System.Threading.Tasks;
using Framework.Scripts.PlayerControl;
using Framework.Scripts.Singleton;
using Pathfinding;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class ObjectManager : ManagerSingleton<ObjectManager>
    {
        public GameObject mainPlayer;
        public static readonly string AnimatorPath = "Sprite/Hero/{0}/{0}_anim/{0}.controller";

        // 加载通过2DUnit，自带Collider2D和SpritRender
        public async Task<GameObject> LoadUnit(object key = null, Transform parent = null, bool instantiate = false)
        {
            GameObject asset;
            if (instantiate)
                asset = await AddressableManager.Instance.Instantiate(Constants.Constants.ObjectUnit, parent);
            else
                asset = await AddressableManager.Instance.LoadAsset<GameObject>(Constants.Constants.ObjectUnit);
            if (key != null)
                asset.GetComponent<SpriteRenderer>().sprite = await AddressableManager.Instance.LoadAsset<Sprite>(key);
            return asset;
        }

        // 创建人物
        public async Task<GameObject> LoadAvatar(object key = null, Transform parent = null)
        {
            GameObject unit = await LoadUnit(key, parent, true);
            // 设置层级
            SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            Rigidbody2D component = (Rigidbody2D) Constants.Constants.AddOrGetComponent(unit, typeof(Rigidbody2D));
            component.angularDrag = 0;
            component.gravityScale = 0;
            // 锁定旋转
            component.constraints = RigidbodyConstraints2D.FreezeRotation;
            BoxCollider2D boxCollider2D = unit.GetComponent<BoxCollider2D>();
            boxCollider2D.size = spriteRenderer.sprite.bounds.size;
            return unit;
        }

        // 创建玩家控制角色
        public async Task LoadPlayerAvatar(string key = null, Transform parent = null)
        {
            if (!LevelManager.Instance.isLevelLoaded)
            {
                Debug.LogError("Level is not load!");
                return;
            }

            string path = String.Format(AnimatorPath, key);
            GameObject unit = await AddressableManager.Instance.Instantiate("AvatarUnit", parent);
            unit.GetComponent<Animator>().runtimeAnimatorController =
                await AddressableManager.Instance.LoadAsset<RuntimeAnimatorController>(path);
            unit.name = key ?? "Avatar";
            // 设置层级
            SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            mainPlayer = unit;
            mainPlayer.name = "MainPlayer";
            // 添加AI寻路组件
            Constants.Constants.AddOrGetComponent(unit, typeof(Seeker));
            AILerp aiLerp = (AILerp) Constants.Constants.AddOrGetComponent(unit, typeof(AILerp));
            aiLerp.orientation = OrientationMode.YAxisForward;
            aiLerp.enableRotation = false;
            aiLerp.enabled = false;
            
            // 添加控制器
            Constants.Constants.AddOrGetComponent(unit, typeof(GamePlayerController));
        }
    }
}