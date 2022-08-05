using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RBossRoomTrigger : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject bossIntroObject = null;
        [SerializeField] private GameObject bossObject = null;

        private const string PLAYER_TAG = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PLAYER_TAG))
            {
                //bossIntroObject.SetActive(true);
            }
        }
    }
}