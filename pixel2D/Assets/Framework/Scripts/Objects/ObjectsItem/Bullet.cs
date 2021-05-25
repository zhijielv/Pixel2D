/*
** Created by fengling
** DateTime:    2021-05-25 15:58:09
** Description: TODO 
*/

using System;
using Framework.Scripts.Manager;
using UnityEngine;

namespace Framework.Scripts.Objects.ObjectsItem
{
    public class Bullet : TrajectoryObject2D
    {
        protected override void OnEnable()
        {
            m_InitializeOnEnable = true;
            base.OnEnable();
        }

        protected override void OnCollision(RaycastHit2D? hit)
        {
            base.OnCollision(hit);
            TimerManager.Instance.Schedule(0, DestroyBullet);
        }

        private void DestroyBullet(object sender, EventArgs e)
        {
            ObjectManager.Instance.Despawn(transform, 1f);
        }
    }
}