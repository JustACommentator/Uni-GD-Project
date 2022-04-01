using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RuneProject.EnemySystem
{
    /// <summary>
    /// 
    /// </summary>
    public class REnemyAI : MonoBehaviour
    {
        [Header("Values")]
        [SerializeField] private Transform goal = null;
        [SerializeField] private float attackDistance = 3;
        [SerializeField] private AlertState currentState = AlertState.IDLE;
        [SerializeField] private float susDistance = 5;

        [Header("References")]
        [SerializeField] private Rigidbody enemyRigidbody = null;
        [SerializeField] private NavMeshAgent agent = null;

        private float susCount = 0;
        private float agressiveness = 0.01f;

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

                    if (Vector3.Distance(enemyRigidbody.position, goal.position) < susDistance)
                    {
                        currentState = AlertState.SUSPICIOUS;
                    }
                    break;

                case AlertState.SUSPICIOUS:

                    if (Vector3.Distance(enemyRigidbody.position, goal.position) < susDistance)
                    {
                        susCount += Mathf.Clamp(susCount + agressiveness * Time.deltaTime, 0, 1);
                    }
                    else
                    {
                        susCount -= Mathf.Clamp(susCount + agressiveness * Time.deltaTime, 0, 1);
                    }

                    if (agressiveness == 1)
                    {
                        currentState = AlertState.AGGRESSIVE;
                    }
                    break;

                case AlertState.AGGRESSIVE:

                    agent.destination = goal.position;

                    if (Vector3.Distance(enemyRigidbody.position, goal.position) < attackDistance)
                    {
                        //DoDamage()
                    }
                    break;
            }
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