/*
** Created by fengling
** DateTime:    2021-04-30 11:18:52
** Description: TODO 管理相机
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using Framework.Scripts.Level;
using Framework.Scripts.Singleton;
using Rewired;
using SRF;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework.Scripts.Manager
{
    public class CameraManager : ManagerSingleton<CameraManager>
    {
        public const float MINOrthographicSize = 3.0f;
        public const float MAXOrthographicSize = 4.9f;
        public GameObject mainCamera;
        public GameObject playerVCamera;
        public GameObject vCameraCollider;
        public GameObject targetGroup;
        public GameObject mouseTarget;

        private List<GameObject> _targetList;
        private const string _followPlayerVCam = "FollowPlayerVCam";

        public override Task Init()
        {
            mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera == null)
            {
                mainCamera = new GameObject();
                mainCamera.AddComponent<Camera>();
                mainCamera.tag = "MainCamera";
                mainCamera.AddComponent<AudioListener>();
                mainCamera.AddComponent<UniversalAdditionalCameraData>();
            }

            mainCamera.transform.SetParent(transform);

            _targetList = new List<GameObject>();
            targetGroup = new GameObject()
            {
                name = "TargetGroup"
            };
            targetGroup.transform.SetParent(transform);
            targetGroup.AddComponent<CinemachineTargetGroup>();
            return base.Init();
        }

        public async void CreatePlayerCamera()
        {
            if (vCameraCollider != null) AddressableManager.Instance.ReleaseInstance(vCameraCollider);
            if (playerVCamera != null) AddressableManager.Instance.ReleaseInstance(playerVCamera);
            // 设置相机边界
            // 获取Level长宽
            int width = LevelManager.Instance.levelLoaderObj.GetComponent<LevelLoader>().level.Width;
            int height = LevelManager.Instance.levelLoaderObj.GetComponent<LevelLoader>().level.Height;
            vCameraCollider = ObjectManager.Instance.LoadUnit(null, LevelManager.Instance.transform, true);
            vCameraCollider.name = "VCamera Collider";
            vCameraCollider.layer = LayerMask.NameToLayer("Ignore Raycast");
            vCameraCollider.transform.SetParent(transform);
            // 设置Collider2D
            Destroy(vCameraCollider.GetComponent<BoxCollider2D>());
            PolygonCollider2D polygonCollider2D = vCameraCollider.AddComponent<PolygonCollider2D>();
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0 - width * Constants.Common.TileSize, 0 + height * Constants.Common.TileSize) /
                        2.0f;
            points[1] = new Vector2(0 - width * Constants.Common.TileSize, 0 - height * Constants.Common.TileSize) /
                        2.0f;
            points[2] = new Vector2(0 + width * Constants.Common.TileSize, 0 - height * Constants.Common.TileSize) /
                        2.0f;
            points[3] = new Vector2(0 + width * Constants.Common.TileSize, 0 + height * Constants.Common.TileSize) /
                        2.0f;
            polygonCollider2D.pathCount = 1;
            polygonCollider2D.SetPath(0, points);
            polygonCollider2D.isTrigger = true;
            // Collider2D设置透明
            vCameraCollider.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            // 设置相机限制范围
            playerVCamera =
                await AddressableManager.Instance.InstantiateAsync(_followPlayerVCam, LevelManager.Instance.transform);
            playerVCamera.transform.SetParent(transform);
            playerVCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = polygonCollider2D;

            // 设置相机跟随
            CinemachineVirtualCamera cinemachineVirtualCamera = playerVCamera.GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = targetGroup.transform;

            // 虚拟相机添加到主相机
            mainCamera.GetComponentOrAdd<CinemachineBrain>();
            // 设置透视
            mainCamera.GetComponent<Camera>().orthographic = true;
        }

        public GameObject CreateMouseTarget()
        {
            GameObject mouseTarget = ObjectManager.Instance.LoadUnit(null, transform, true);
            Destroy(mouseTarget.GetComponent<BoxCollider2D>());
            _targetList.Add(mouseTarget);
            ResetTargetList();
            return mouseTarget;
        }

        public void AddTarget(GameObject target)
        {
            _targetList.Add(target);
            ResetTargetList();
        }

        public void RemoveTarget(GameObject target)
        {
            _targetList.Remove(target);
        }

        public void CameraFollowMouse()
        {
            mouseTarget = CreateMouseTarget();
            mouseTarget.GetComponent<SpriteRenderer>().enabled = false;
            RewiredInputEventManager.Instance.AddEvent(SetMouseTarget, UpdateLoopType.Update,
                InputActionEventType.AxisActive, "MouseHorizontal");
            RewiredInputEventManager.Instance.AddEvent(SetMouseTarget, UpdateLoopType.Update,
                InputActionEventType.AxisActive, "MouseVertical");
        }

        private void SetMouseTarget(InputActionEventData inputActionEventData)
        {
            // 镜头移动
            
            if (mouseTarget == null) return;
            if (playerVCamera == null) return;
            CinemachineFramingTransposer vcam =
                playerVCamera
                    .GetComponent<CinemachineVirtualCamera>()
                    .GetCinemachineComponent<CinemachineFramingTransposer>();
            float cameraDistance = vcam.m_CameraDistance;
            Vector2 mouseScreenPosition = RewiredInputEventManager.Instance.player0.controllers.Mouse.screenPosition;
            Vector3 point =
                Camera.main.ScreenToWorldPoint(
                    new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, cameraDistance));
            point.z = 0;
            mouseTarget.transform.position = point;
            mouseTarget.GetComponent<SpriteRenderer>().sortingOrder = -1;
        }

        private void ResetTargetList()
        {
            CinemachineTargetGroup cinemachineTargetGroup = targetGroup.GetComponent<CinemachineTargetGroup>();
            cinemachineTargetGroup.m_Targets = new CinemachineTargetGroup.Target[0];
            foreach (var t in _targetList)
            {
                cinemachineTargetGroup.AddMember(t.transform, 1, 0);
            }
        }
    }
}