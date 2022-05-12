using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RPlayerSpawnPoint : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject playerPrefab = null;
        [Space]
        [SerializeField] private ParticleSystem onSpawnParticleSystem = null;

        [Header("Values")]
        [SerializeField] private bool spawnPlayerOnStart = true;
        [SerializeField] private Vector3 playerSpawnOffset = Vector3.up;
        [SerializeField] private Vector3 playerSpawnEulerOffset = Vector3.zero;

        private void Start()
        {
            if (spawnPlayerOnStart)
                SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            Instantiate(playerPrefab, transform.position + playerSpawnOffset, Quaternion.Euler(transform.eulerAngles + playerSpawnEulerOffset));
            onSpawnParticleSystem.Play();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + playerSpawnOffset, 0.1f);
        }
    }
}