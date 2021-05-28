/*
** Created by fengling
** DateTime:    2021-05-25 15:58:09
** Description: 
*/

using Framework.Scripts.Manager;
using Framework.Scripts.PlayerControl;
using UnityEngine;
using EventType = Framework.Scripts.Constants.EventType;

namespace Framework.Scripts.Objects.ObjectsItem
{
    public class Bullet : TrajectoryObject2D
    {
        public Vector3 velocity;

        // 延迟多少秒后销毁
        public float destroyDelay = 0.5f;
        public void Fire(Vector3 velocity)
        {
            Initialize(velocity, Vector3.zero, gameObject);
        }

        protected override void Awake()
        {
            base.Awake();
            m_ImpactLayers |= 1 << LayerMask.NameToLayer("Box") | 1 << LayerMask.NameToLayer("Enemies");
        }

        public override void Initialize(Vector3 velocity, Vector3 torque, GameObject originator,
            bool originatorCollisionCheck)
        {
            m_Originator = originator;
            m_OriginatorTransform = m_Originator.transform;
            base.Initialize(velocity, torque, originator, originatorCollisionCheck);
        }

        protected override void OnCollision(RaycastHit2D? hit)
        {
            base.OnCollision(hit);
            GameWeaponController gameWeaponController = ObjectManager.Instance.mainPlayer.transform.GetChild(0).GetComponent<GameWeaponController>();
            
            // 事件调用
            EventManager.Instance.DispatchEvent(gameWeaponController.transform, EventType.PlayerBulletCollide, new EventData<float>(destroyDelay){Data = transform});
        }
    }
}