using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    public class RMoveableBlockComponent : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ParticleSystem pushParticleSystem = null;
        [SerializeField] private AudioSource pushAudioSource = null;

        [Header("Values")]
        [SerializeField] private int maxPushDistance = 1;
        [SerializeField] private LayerMask pushBlockMask = new LayerMask();

        private Coroutine currentPushRoutine = null;

        private const float PUSH_TIME = 0.3f;
        private const float PARTICLE_SYSTEM_DISTANCE = 0.375f;

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player") && CanPush())
            {
                currentPushRoutine = StartCoroutine(IExecutePush(transform.position - collision.collider.transform.position, 
                    collision.collider.GetComponent<RPlayerMovement>()));
            }
        }

        private bool CanPush()
        {
            return currentPushRoutine == null;
        }

        private IEnumerator IExecutePush(Vector3 direction, RPlayerMovement movement)
        {
            Vector2 dir = new Vector2(direction.x, direction.z);

            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))            
                dir.y = 0f;            
            else            
                dir.x = 0f;            

            dir.Normalize();

            float hitDistance = maxPushDistance;
            Vector3 dir3 = new Vector3(dir.x, 0f, dir.y);

            RaycastHit[] hits = Physics.RaycastAll(transform.position + Vector3.up * 0.2f, dir3, float.PositiveInfinity, pushBlockMask);

            for (int i = 0; i < hits.Length; i++)
            {
                float distance = Mathf.FloorToInt(Vector3.Distance(hits[i].point, transform.position));
                if (distance < hitDistance)
                    hitDistance = distance;
            }

            if (hitDistance > 0)
            {
                Vector3 startPos = transform.position;
                Vector3 targetPos = transform.position + dir3 * hitDistance;

                movement.SignalPushLevelObject(gameObject);
                movement.BlockMovementInput(PUSH_TIME);
                pushParticleSystem.transform.localPosition = dir3 * PARTICLE_SYSTEM_DISTANCE;
                pushParticleSystem.transform.localEulerAngles = new Vector3(pushParticleSystem.transform.localEulerAngles.x, dir.x < 0f ? 90f : dir.x > 0f ? 270f : dir.y < 0f ? 0f : 180f, pushParticleSystem.transform.localEulerAngles.z);
                pushParticleSystem.Play();
                pushAudioSource.Play();

                float timer = 0f;
                while(transform.position != targetPos)
                {
                    timer += Time.deltaTime;
                    transform.position = Vector3.Lerp(startPos, targetPos, timer / PUSH_TIME);
                    yield return null;
                }
            }

            currentPushRoutine = null;
        }
    }
}