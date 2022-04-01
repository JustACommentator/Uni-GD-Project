using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RuneProject.EnemySystem
{
    /// <summary>
    /// Controlls the states of the enemy and makes it attack the target.
    /// </summary>
    public class REnemyAI : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private Transform target = null;
        [SerializeField] private float attackDistance = 3;
        [SerializeField] private AlertState currentState = AlertState.IDLE;
        [SerializeField] private float susDistance = 5;
        /// <summary>
        /// Time in seconds before the enemy attacks.
        /// </summary>
        [SerializeField] private float aggroTime = 3;

        [Header("References")]
        [SerializeField] private Rigidbody enemyRigidbody = null;
        [SerializeField] private NavMeshAgent agent = null;

        private float susCount = 0;

        enum AlertState
        {
            IDLE,
            SUSPICIOUS,
            AGGRESSIVE
        }

        void Update()
        {
            switch (currentState)
            {
                case AlertState.IDLE:

                    if (Vector3.Distance(enemyRigidbody.position, target.position) < susDistance)
                    {
                        currentState = AlertState.SUSPICIOUS;
                    }
                    break;

                case AlertState.SUSPICIOUS:

                    if (Vector3.Distance(enemyRigidbody.position, target.position) < susDistance)
                    {
                        susCount = Mathf.Clamp(susCount + (1 / aggroTime) * Time.deltaTime, 0, 1);
                    }
                    else
                    {
                        susCount = Mathf.Clamp(susCount - (1 / aggroTime) * Time.deltaTime, 0, 1);
                    }

                    if (susCount == 1)
                    {
                        currentState = AlertState.AGGRESSIVE;
                    }
                    break;

                case AlertState.AGGRESSIVE:

                    agent.destination = target.position;

                    if (Vector3.Distance(enemyRigidbody.position, target.position) < attackDistance)
                    {
                        //DoDamage()
                    }
                    break;
            }

            Debug.Log(currentState);
            Debug.Log(susCount);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, susDistance);
        }
    }
}