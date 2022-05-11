using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RuneProject.EnemySystem
{
    public class REnemyRagdollDeath : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RPlayerHealth enemyHealth = null;
        [SerializeField] private REnemyAI enemyAI = null;
        [SerializeField] private NavMeshAgent enemyAgent = null;
        [SerializeField] private Rigidbody enemyRigidbody = null;
        [SerializeField] private List<Collider> enemyColliders = new List<Collider>();
        [Space]
        [SerializeField] private PhysicMaterial bouncyMaterial = null;
        [SerializeField] private ParticleSystem destroyParticleSystemPrefab = null;

        [Header("Values")]
        [SerializeField] private float ragdollInitialBurstPower = 2000f;

        private bool isPlaying = false;

        private const int DONT_COLLIDE_WITH_ENTITIES_LAYER = 8;
        private const float MIN_VELOCITY_BEFORE_DESTROY = 0.1f;
        private const float MIN_FLY_TIME = 0.5f;
        private const float OVERRIDE_DRAG = 0.2f;
        private const float DESTROY_DELAY = 0.1f;
        private const float COLLISION_VELOCITY_STRENGTH = 0.5f;
        private const float PARTICLE_LIFETIME = 2f;

        private void Start()
        {
            enemyHealth.OnDeath += EnemyHealth_OnDeath;
        }

        private void OnDestroy()
        {
            enemyHealth.OnDeath -= EnemyHealth_OnDeath;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isPlaying)            
                enemyRigidbody.AddForce(COLLISION_VELOCITY_STRENGTH * enemyRigidbody.velocity.magnitude * (Random.Range(0, 2) == 0 ? transform.right : -transform.right));            
        }

        private void EnemyHealth_OnDeath(object sender, GameObject e)
        {
            if (isPlaying) return;

            isPlaying = true;

            enemyAI.enabled = false;
            enemyAgent.enabled = false;
            enemyHealth.gameObject.layer = DONT_COLLIDE_WITH_ENTITIES_LAYER;

            for (int i = 0; i < enemyColliders.Count; i++)
                enemyColliders[i].material = bouncyMaterial;

            enemyRigidbody.velocity = Vector3.zero;
            Vector3 ownerPos = transform.position;
            ownerPos.y = 0f;
            Vector3 killerPos = e.transform.position;
            killerPos.y = 0f;
            Vector3 dir = (ownerPos - killerPos).normalized;
            enemyRigidbody.drag = OVERRIDE_DRAG;
            enemyRigidbody.AddForce(dir * ragdollInitialBurstPower * enemyHealth.ReceivedKnockbackMultiplier);

            StartCoroutine(IDestroyEnemyOnVelocityReachZero());
        }

        private IEnumerator IDestroyEnemyOnVelocityReachZero()
        {
            yield return new WaitForSeconds(MIN_FLY_TIME);

            while (enemyRigidbody.velocity.magnitude > MIN_VELOCITY_BEFORE_DESTROY)
                yield return null;

            Destroy(Instantiate(destroyParticleSystemPrefab, transform.position, Quaternion.identity), PARTICLE_LIFETIME);
            Destroy(gameObject, DESTROY_DELAY);
        }
    }
}