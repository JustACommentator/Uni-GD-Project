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
        [SerializeField] private PatrolMode currentPatrolState = PatrolMode.DEFAULT;
        [SerializeField] private float susDistance = 5;
        [SerializeField] private float aggroTime = 3;

        [Header("Patrol")]
        [SerializeField] private List<Transform> path = null;

        [Header("References")]
        [SerializeField] private Rigidbody enemyRigidbody = null;
        [SerializeField] private NavMeshAgent agent = null;

        private float susCount = 0;
        private int currentPathPoint = 0;
        private float reachedDistance = 1;

        enum AlertState
        {
            IDLE,
            SUSPICIOUS,
            AGGRESSIVE
        }

        enum PatrolMode
        {
            DEFAULT,
            TRACE_BACK,
            PING_PONG,
            RANDOM
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

                    if (path != null && path.Count > 1)
                    {
                        switch (currentPatrolState)
                        {
                            case PatrolMode.DEFAULT:

                                agent.destination = path[currentPathPoint].position;

                                if (Vector3.Distance(enemyRigidbody.position, path[currentPathPoint].position) < reachedDistance)
                                {
                                    currentPathPoint = (currentPathPoint + 1) % path.Count;
                                }

                                break;

                            case PatrolMode.TRACE_BACK:

                                agent.destination = path[Mathf.Abs(currentPathPoint - path.Count) - path.Count].position;

                                if (Vector3.Distance(enemyRigidbody.position, path[currentPathPoint].position) < reachedDistance)
                                {
                                    currentPathPoint = (currentPathPoint + 1) % (path.Count * 2);
                                }
                                break;

                            case PatrolMode.PING_PONG:

                                agent.destination = path[currentPathPoint].position;

                                if (Vector3.Distance(enemyRigidbody.position, path[currentPathPoint].position) < reachedDistance)
                                {
                                    if (currentPathPoint == 0)
                                    {
                                        currentPathPoint = Random.Range(1, path.Count);
                                    }
                                    else
                                    {
                                        currentPathPoint = 0;
                                    }
                                }

                                break;

                            case PatrolMode.RANDOM:

                                agent.destination = path[currentPathPoint].position;

                                if (Vector3.Distance(enemyRigidbody.position, path[currentPathPoint].position) < reachedDistance)
                                {
                                    currentPathPoint = Random.Range(0, path.Count);
                                }

                                break;
                        }
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
            Debug.Log(currentPathPoint);
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