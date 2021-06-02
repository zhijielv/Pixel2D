/*
** Created by fengling
** DateTime:    2021-05-15 10:16:09
** Description: 武器控制
*/

using System;
using System.Collections.Generic;
using Framework.Scripts.Manager;
using Framework.Scripts.Objects.ObjectsItem;
using Framework.Scripts.Utils;
using PathologicalGames;
using Rewired;
using Sirenix.Utilities;
using UnityEngine;
using EventType = Framework.Scripts.Constants.EventType;

namespace Framework.Scripts.PlayerControl
{
    public class GameWeaponController : MonoBehaviour
    {
        public string weaponName = "1";
        public GameObject currentWeapon;
        public float bulletSpeed = 10;
        public Vector3 velocity;
        public List<GameObject> weaponList;

        public Transform bullet;
        public SpawnPool bulletPool;

        // todo 当前子弹类型
        public Sprite currentBullet;

        private IntervalEvent intervalEvent;
        private async void Start()
        {
            weaponList = new List<GameObject>();
            // 加载武器
            string path = String.Format(Constants.Constants.WeaponPath, weaponName);
            currentWeapon = ObjectManager.Instance.LoadUnit(path, transform, true);
            SpriteRenderer spriteRenderer = currentWeapon.GetComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 1;
            Vector3 offset =
                new Vector3(
                    (spriteRenderer.sprite.bounds.size.x + ObjectManager.Instance.mainPlayer
                        .GetComponent<SpriteRenderer>().sprite.bounds.size.x) / 2.0f, 0, 0);
            currentWeapon.transform.localPosition = offset;
            currentWeapon.GetComponent<BoxCollider2D>().isTrigger = true;
            weaponList.Add(currentWeapon);

            // 设置事件
            RewiredInputEventManager.Instance.AddEvent(OnFireButtonClick, UpdateLoopType.Update,
                InputActionEventType.ButtonJustPressed, "Fire");
            // RewiredInputEventManager.Instance.AddEvent(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Fire");

            // 设置武器旋转
            RewiredInputEventManager.Instance.AddEvent(SetMouseTarget, UpdateLoopType.Update,
                InputActionEventType.AxisActive, "MouseHorizontal");
            RewiredInputEventManager.Instance.AddEvent(SetMouseTarget, UpdateLoopType.Update,
                InputActionEventType.AxisActive, "MouseVertical");

            // 设置子弹池
            RegistBullet();
        }

        private void OnEnable()
        {
            // 注册事件

            EventManager.Instance.AddEventListener(transform, EventType.PlayerBulletCollide, OnBulletCollideHandler);
            
            // 开火 使用间隔事件（带CD的时间）
            // todo 武器cd
            intervalEvent = new IntervalEvent(2);
            EventManager.Instance.AddEventListener(transform, EventType.PlayerWeaponFire, WeaponFire);
            
            GlobalConfig<LevelHelper>.Instance.weaponController = this;
        }

        private void WeaponFire(object sender, EventArgs e)
        {
            if (null == currentWeapon || weaponList.Count == 0) return;
            Transform spawnObj =
                bulletPool.Spawn(bullet, currentWeapon.transform.position, currentWeapon.transform.rotation);
            velocity = transform.right.normalized * bulletSpeed;

            spawnObj.GetComponent<Bullet>().Fire(velocity);
        }

        // 武器跟随鼠标旋转
        private void SetMouseTarget(InputActionEventData inputActionEventData)
        {
            if (null == currentWeapon || weaponList.Count == 0) return;
            var playerPosition = ObjectManager.Instance.mainPlayer.transform.position;

            Vector3 targetVector3 = (CameraManager.Instance.mouseTarget.transform.position
                                     - playerPosition).normalized;

            Quaternion quaternion = Quaternion.FromToRotation(Vector3.right, targetVector3);
            transform.localRotation = quaternion;

            currentWeapon.GetComponent<SpriteRenderer>().flipY = Vector3.Cross(targetVector3, Vector3.up).z < 0;
        }

        // 左键攻击，对象池创建子弹
        public void OnFireButtonClick(InputActionEventData data)
        {
            EventManager.Instance.DispatchEvent(transform, EventType.PlayerWeaponFire, intervalEvent);
            // if (null == currentWeapon || weaponList.Count == 0) return;
            // Transform spawnObj =
            //     bulletPool.Spawn(bullet, currentWeapon.transform.position, currentWeapon.transform.rotation);
            // velocity = transform.right.normalized * bulletSpeed;
            //
            // spawnObj.GetComponent<Bullet>().Fire(velocity);
        }
        
        // 子弹碰撞后回池
        private void OnBulletCollideHandler(object sender, EventArgs e)
        {
            EventData<float> eventData = e as EventData<float>;
            if (eventData != null) bulletPool.Despawn(eventData.Data as Transform, eventData.Value);
        }

        // todo 子弹类型
        public void RegistBullet()
        {
            bullet = AddressableManager.Instance.LoadAsset<GameObject>("PlayerBullet")
                .transform;
            bulletPool = ObjectManager.Instance.RegisterPool(bullet);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveEventListener(transform, EventType.PlayerBulletCollide, OnBulletCollideHandler);
            EventManager.Instance.RemoveEventListener(transform, EventType.PlayerWeaponFire, WeaponFire);
        }
    }
}