using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class REnvironmentKeyComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool isBossKey = false;

        [Header("Prefabs")]
        [SerializeField] private AudioSource collectKeyAudioSourcePrefab = null;
        [SerializeField] private ParticleSystem collectKeyParticleSystemPrefab = null;

        private bool wasCollected = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !wasCollected)
            {
                wasCollected = true;

                RPlayerInventory otherInventory = other.GetComponent<RPlayerInventory>();

                if (isBossKey)
                    otherInventory.CurrentBossKeys += 1;
                else
                    otherInventory.CurrentKeys += 1;

                Instantiate(collectKeyAudioSourcePrefab, transform.position, Quaternion.identity);
                Instantiate(collectKeyParticleSystemPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }
}