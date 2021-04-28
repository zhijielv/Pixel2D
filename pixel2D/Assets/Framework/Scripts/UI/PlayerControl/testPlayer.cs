/*
** Created by fengling
** DateTime:    2021-04-28 14:35:16
** Description: TODO 
*/

using System;
using UnityEngine;

namespace Framework.Scripts.UI.PlayerControl
{
    public class testPlayer : MonoBehaviour
    {
        public GameObject Player;

        private void OnCollisionEnter2D(Collision2D other)
        {
            Debug.Log(other.transform.name);
        }

        private void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.name);
        }
    }
}