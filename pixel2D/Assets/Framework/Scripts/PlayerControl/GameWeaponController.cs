/*
** Created by fengling
** DateTime:    2021-05-15 10:16:09
** Description: 武器控制
*/

using System;
using System.Collections.Generic;
using Framework.Scripts.Manager;
using Rewired;
using UnityEngine;

namespace Framework.Scripts.PlayerControl
{
    public class GameWeaponController : MonoBehaviour
    {
        public string weaponName = "1";
        public GameObject currentWeapon;
        public List<GameObject> weaponList;

        private async void Start()
        {
            weaponList = new List<GameObject>();
            // 加载武器
            string path = String.Format(Constants.Constants.WeaponPath, weaponName);
            currentWeapon = await ObjectManager.Instance.LoadUnit(path, transform, true);
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

        // 攻击
        public void OnFireButtonClick(InputActionEventData data)
        {
            if (null == currentWeapon || weaponList.Count == 0) return;
            Debug.Log($"Button Fire!  {data.GetButton()}");
            // EventManager.Instance.DispatchEvent(EventConstants.StartGame);
        }
    }
}