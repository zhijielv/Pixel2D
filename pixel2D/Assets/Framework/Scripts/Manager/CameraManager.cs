/*
** Created by fengling
** DateTime:    2021-04-30 11:18:52
** Description: TODO 管理相机
*/

using System.Threading.Tasks;
using Cinemachine;
using Framework.Scripts.Level;
using Framework.Scripts.Singleton;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework.Scripts.Manager
{
    public class CameraManager : ManagerSingleton<CameraManager>
    {
        public GameObject mainCamera;
        public GameObject playerVCamera;

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

            return base.Init();
        }

        public async void CreatePlayerCamera()
        {
            // 设置相机边界
            // 获取Level长宽
            int width = LevelManager.Instance.levelLoaderObj.GetComponent<LevelLoader>().level.Width;
            int height = LevelManager.Instance.levelLoaderObj.GetComponent<LevelLoader>().level.Height;
            GameObject vCameraCollider =
                await ObjectManager.Instance.LoadUnit(null, LevelManager.Instance.transform, true);
            vCameraCollider.name = "VCamera Collider";
            // 设置Collider2D
            Destroy(vCameraCollider.GetComponent<BoxCollider2D>());
            PolygonCollider2D polygonCollider2D = vCameraCollider.AddComponent<PolygonCollider2D>();
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(0 - width * Constants.Common.TileSize, 0 + height * Constants.Common.TileSize) / 2.0f;
            points[1] = new Vector2(0 - width * Constants.Common.TileSize, 0 - height * Constants.Common.TileSize) / 2.0f;
            points[2] = new Vector2(0 + width * Constants.Common.TileSize, 0 - height * Constants.Common.TileSize) / 2.0f;
            points[3] = new Vector2(0 + width * Constants.Common.TileSize, 0 + height * Constants.Common.TileSize) / 2.0f;
            polygonCollider2D.pathCount = 1;
            polygonCollider2D.SetPath(0, points);
            polygonCollider2D.isTrigger = true;
            // Collider2D设置透明
            vCameraCollider.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            // 设置相机跟随
            playerVCamera =
                await AddressableManager.Instance.Instantiate(_followPlayerVCam, LevelManager.Instance.transform);
            CinemachineVirtualCamera cinemachineVirtualCamera = playerVCamera.GetComponent<CinemachineVirtualCamera>();
            cinemachineVirtualCamera.Follow = ObjectManager.Instance.MainPlayer.transform;
            cinemachineVirtualCamera.LookAt = ObjectManager.Instance.MainPlayer.transform;
            // playerVCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = boxCollider2D;
            playerVCamera.GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = polygonCollider2D;

            // 虚拟相机添加到主相机
            Constants.Constants.AddOrGetComponent(mainCamera, typeof(CinemachineBrain));
            // 设置透视
            mainCamera.GetComponent<Camera>().orthographic = false;
        }
    }
}