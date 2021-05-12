/*
** Created by fengling
** DateTime:    2021-05-12 15:24:40
** Description: 玩家控制器 
*/

using Framework.Scripts.Manager;
using Pathfinding;
using Rewired;
using UnityEngine;

namespace Framework.Scripts.PlayerControl
{
    public class GamePlayerController : MonoBehaviour
    {
        private void Awake()
        {
            CameraManager.Instance.CameraFollowMouse();
            RewiredInputEventManager.Instance.AddEvent(MoveX, UpdateLoopType.Update, InputActionEventType.AxisActive, "MoveX");
            RewiredInputEventManager.Instance.AddEvent(OnFireButtonClick, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "Fire");
            // RewiredInputEventManager.Instance.AddEvent(TestButton, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased, "Fire");
            
            // 右键地面寻路功能
            // RewiredInputEventManager.Instance.AddEvent(TestAStar, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed, "MoveToClick");

        }
        
        public void MoveX(InputActionEventData data)
        {
            if(ObjectManager.Instance.mainPlayer == null) return;
            float direction = data.GetAxis() > 0 ? 1 : -1;
            ObjectManager.Instance.mainPlayer.transform.localScale = new Vector3(direction, 1, 1);
        }
        
        public void OnFireButtonClick(InputActionEventData data)
        {
            Debug.Log($"Button Fire!  {data.GetButton()}");
            // EventManager.Instance.DispatchEvent(EventConstants.StartGame);
        }
        
        // 右键点击地面寻路
        public void TestAStar(InputActionEventData data)
        {
            if(!LevelManager.Instance.isLevelLoaded) return;
            if(!RewiredInputEventManager.Instance.player0.controllers.hasMouse) return;
            Seeker seeker = ObjectManager.Instance.mainPlayer.GetComponent<Seeker>();
            AILerp aiLerp = ObjectManager.Instance.mainPlayer.GetComponent<AILerp>();
            aiLerp.enabled = true;
            Vector2 mouseScreenPosition = RewiredInputEventManager.Instance.player0.controllers.Mouse.screenPosition;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0));
            Vector3 targetPosition = new Vector3(worldPoint.x, worldPoint.y, seeker.transform.position.z);
            seeker.StartPath(seeker.transform.position, targetPosition);
        }
    }
}