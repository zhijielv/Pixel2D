/*
** Created by fengling
** DateTime:    2021-04-28 14:35:16
** Description: 
*/

using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Framework.Scripts.UI.PlayerControl
{
    public class TestPlayer : MonoBehaviour
    {
        [FormerlySerializedAs("Player")] public GameObject player;

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