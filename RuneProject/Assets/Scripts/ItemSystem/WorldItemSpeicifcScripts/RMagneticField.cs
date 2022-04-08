using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RuneProject.ItemSystem
{
    /// <summary>
    /// A magnetic field will suck in everything it touches.
    /// </summary>
    public class RMagneticField : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private float attractionForce = 10f;
        [SerializeField] private float minDistanceToApplyForce = 0.1f;

        private List<Rigidbody> currentTargets = new List<Rigidbody>();
        private List<NavMeshAgent> currentNavmeshAgentTargets = new List<NavMeshAgent>();

        private void FixedUpdate()
        {
            for (int i=0; i<currentTargets.Count; i++)
            {
                Transform current = currentTargets[i].transform;
                if (Vector3.Distance(transform.position, current.position) > minDistanceToApplyForce)
                    currentTargets[i].AddForce((transform.position - current.position).normalized * attractionForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Rigidbody>(out Rigidbody otherRB))
            {
                otherRB.velocity = Vector3.zero;
                currentTargets.Add(otherRB);
            }

            if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent otherAgent))
            {
                currentNavmeshAgentTargets.Add(otherAgent);
                otherAgent.enabled = false;

                if (otherAgent.isOnNavMesh)
                    otherAgent.isStopped = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<Rigidbody>(out Rigidbody otherRB) && currentTargets.Contains(otherRB))
            {
                currentTargets.Remove(otherRB);
            }

            if (other.TryGetComponent<NavMeshAgent>(out NavMeshAgent otherAgent) && currentNavmeshAgentTargets.Contains(otherAgent))
            {
                otherAgent.enabled = true;
                otherAgent.isStopped = false;
            }            
        }

        private void OnDestroy()
        {
            if (enabled)
                ReleaseAll();
        }

        /// <summary>
        /// Releases all current targets.
        /// </summary>
        public void ReleaseAll()
        {
            for (int i = 0; i < currentNavmeshAgentTargets.Count; i++)
            {
                if (currentNavmeshAgentTargets[i])
                {
                    currentNavmeshAgentTargets[i].enabled = true;
                    currentNavmeshAgentTargets[i].isStopped = false;
                }
            }

            currentTargets.Clear();
            currentNavmeshAgentTargets.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, minDistanceToApplyForce);
        }
    }
}