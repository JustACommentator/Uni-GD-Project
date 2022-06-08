using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RKeyLockedDoorComponent : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private bool needsBossKeys = false;
        [SerializeField] private int neededKeyCount = 1;

        [Header("References")]
        [SerializeField] private AudioSource unlockSoundSource = null;
        [SerializeField] private ParticleSystem unlockParticleSystem = null;
        [SerializeField] private List<GameObject> lockedToBeDisabled = new List<GameObject>();

        private Coroutine currentUnlockRoutine = null;

        private const float OPEN_TIME = 0.6f;       

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                RPlayerInventory inv = collision.collider.GetComponent<RPlayerInventory>();

                if (needsBossKeys)
                {
                    if (inv.CurrentBossKeys >= neededKeyCount)
                    {
                        inv.CurrentBossKeys -= neededKeyCount;
                        Unlock(inv.GetComponent<RPlayerMovement>());
                    }
                }
                else
                {
                    if (inv.CurrentKeys >= neededKeyCount)
                    {
                        inv.CurrentKeys -= neededKeyCount;
                        Unlock(inv.GetComponent<RPlayerMovement>());
                    }
                }
            }
        }

        private void Unlock(RPlayerMovement movement)
        {
            if (currentUnlockRoutine == null)
            {
                movement.BlockMovementInput(OPEN_TIME);
                currentUnlockRoutine = StartCoroutine(IExecuteUnlock());
            }
        }

        private IEnumerator IExecuteUnlock()
        {
            unlockSoundSource.Play();
            yield return new WaitForSeconds(OPEN_TIME);

            unlockParticleSystem.Play();

            for (int i=0; i< lockedToBeDisabled.Count; i++)
                lockedToBeDisabled[i].SetActive(false);
        }
    }
}