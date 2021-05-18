/*
** Created by fengling
** DateTime:    2021-04-28 17:10:19
** Description: TODO 对象池
*/

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Scripts.PlayerControl;
using Framework.Scripts.Singleton;
using Pathfinding;
using PathologicalGames;
using SRF;
using UnityEngine;

namespace Framework.Scripts.Manager
{
    public class ObjectManager : ManagerSingleton<ObjectManager>
    {
        public GameObject mainPlayer;
        public SpawnPool unitPool;
        public Transform objectUnit;
        public Dictionary<string, SpawnPool> poolDic;

        public override Task Init()
        {
            poolDic = new Dictionary<string, SpawnPool>();
            objectUnit = LoadUnit().transform;
            unitPool = RegisterPool(objectUnit);
            return base.Init();
        }

        #region Load

        // 加载通过2DUnit，自带Collider2D和SpritRender
        public GameObject LoadUnit(object key = null, Transform parent = null, bool instantiate = false)
        {
            GameObject asset;
            asset = instantiate
                ? AddressableManager.Instance.Instantiate(Constants.Constants.ObjectUnit, parent)
                : AddressableManager.Instance.LoadAsset<GameObject>(Constants.Constants.ObjectUnit);
            if (key != null)
                asset.GetComponent<SpriteRenderer>().sprite = AddressableManager.Instance.LoadAsset<Sprite>(key);
            return asset;
        }

        // 创建人物
        public GameObject LoadAvatar(object key = null, Transform parent = null)
        {
            GameObject unit = LoadUnit(key, parent, true);
            // 设置层级
            SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            Rigidbody2D component = unit.GetComponentOrAdd<Rigidbody2D>();
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

            string path = String.Format(Constants.Constants.AnimatorPath, key);
            GameObject unit = await AddressableManager.Instance.InstantiateAsync("AvatarUnit", parent);
            unit.GetComponent<Animator>().runtimeAnimatorController =
                await AddressableManager.Instance.LoadAssetAsync<RuntimeAnimatorController>(path);
            unit.name = key ?? "Avatar";
            // 设置层级
            SpriteRenderer spriteRenderer = unit.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            mainPlayer = unit;
            mainPlayer.name = "MainPlayer";
            // 添加AI寻路组件
            unit.GetComponentOrAdd<Seeker>();
            AILerp aiLerp = unit.GetComponentOrAdd<AILerp>();
            aiLerp.orientation = OrientationMode.YAxisForward;
            aiLerp.enableRotation = false;
            aiLerp.enabled = false;

            // 添加控制器
            unit.GetComponentOrAdd<GamePlayerController>();
        }

        #endregion

        #region ObjectPool

        public SpawnPool RegisterPool(Transform prefabTransform)
        {
            GameObject o = new GameObject(prefabTransform.name + "_Pool");
            o.transform.SetParent(transform);
            SpawnPool spawnPool = PoolManager.Pools.Create(o.name, o);
            PrefabPool prefabPool = new PrefabPool(prefabTransform);
            spawnPool._perPrefabPoolOptions.Add(prefabPool);
            spawnPool.CreatePrefabPool(prefabPool);
            poolDic.Add(spawnPool.poolName, spawnPool);
            return spawnPool;
        }

        public Transform SpawnUnit(float seconds = 0f)
        {
            Transform spawn = unitPool.Spawn(objectUnit);
            if (seconds > 0)
                unitPool.Despawn(spawn, seconds);
            return spawn;
        }
        
        public Transform Spawn(SpawnPool spawnPool, Transform prefabTransform, float seconds = 0f)
        {
            Transform spawn = spawnPool.Spawn(prefabTransform);
            if (seconds > 0)
                spawnPool.Despawn(spawn, seconds);
            return spawn;
        }

        #endregion
    }
}