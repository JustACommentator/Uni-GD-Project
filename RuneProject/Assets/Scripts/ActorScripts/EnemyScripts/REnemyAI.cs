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
        [SerializeField] private float attackDistance = 2;
        [SerializeField] private AlertState currentState = AlertState.IDLE;
        [SerializeField] private PatrolMode currentPatrolState = PatrolMode.DEFAULT;
        [SerializeField] private float susDistance = 5;
        [SerializeField] private float aggroTime = 3;

        [Header("Patrol")]
        [SerializeField] private List<Transform> path = new List<Transform>();
        [SerializeField] private bool startAtFirstPathPosition = true;
        [SerializeField] private float reachedDistance = 1;

        [Header("References")]
        [SerializeField] private NavMeshAgent agent = null;

        private float susCount = 0;
        private int currentPathPoint = 0;
        private bool pathForward = true;



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

        private void Start()
        {
            if (path.Count > 1 && startAtFirstPathPosition)
                transform.position = path[0].position;
        }

        private void Update()
        {
            switch (currentState)
            {
                case AlertState.IDLE:

                    if (Vector3.Distance(transform.position, target.position) < susDistance)
                    {
                        currentState = AlertState.SUSPICIOUS;
                        return;
                    }

                    if (path.Count > 1 && Vector3.Distance(transform.position, path[currentPathPoint].position) <= reachedDistance)
                    {

                        Debug.Log(currentPathPoint);
                        switch (currentPatrolState)
                        {
                            case PatrolMode.DEFAULT:
                                currentPathPoint = (currentPathPoint + 1) % path.Count;                                    
                                break;

                            case PatrolMode.TRACE_BACK:
                                if (pathForward)
                                {
                                    currentPathPoint++;
                                }
                                else
                                {
                                    currentPathPoint--;
                                }

                                if (currentPathPoint >= path.Count - 1)
                                {
                                    pathForward = false;
                                }

                                if (currentPathPoint <= 0)
                                {
                                    pathForward = true;
                                }
                                break;

                            case PatrolMode.PING_PONG:
                                if (currentPathPoint == 0)                                        
                                    currentPathPoint = Random.Range(1, path.Count);                                        
                                else                                        
                                    currentPathPoint = 0;                                                                           
                                break;

                            case PatrolMode.RANDOM:
                                int newPathPoint = Random.Range(0, path.Count);
                                while (currentPathPoint == newPathPoint)
                                {
                                    newPathPoint = Random.Range(0, path.Count);
                                }
                                currentPathPoint = newPathPoint;
                                break;
                        }

                        agent.SetDestination(path[currentPathPoint].position);
                    }                    

                    break;

                case AlertState.SUSPICIOUS:

                    if (Vector3.Distance(transform.position, target.position) < susDistance)
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

                    if (Vector3.Distance(transform.position, target.position) < attackDistance)
                    {
                        //DoDamage()
                    }
                    else
                    {
                        agent.destination = target.position;
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

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * reachedDistance);

            if (path.Count > 1)
                Gizmos.DrawLine(transform.position, path[currentPathPoint].position);

            switch(currentPatrolState)
            {
                case PatrolMode.DEFAULT:
                    for (int i = 0; i < path.Count; i++)
                    {
                        Gizmos.DrawLine(path[i].position, path[(i + 1) % path.Count].position);
                    }
                    break;

                case PatrolMode.TRACE_BACK:
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Gizmos.DrawLine(path[i].position, path[(i + 1) % path.Count].position);
                    }
                    break;

                case PatrolMode.PING_PONG:
                    for (int i = 0; i < path.Count; i++)
                    {
                        Gizmos.DrawLine(path[0].position, path[i].position);
                    }
                    break;

                case PatrolMode.RANDOM:
                    for (int i = 0; i < path.Count; i++)
                    {
                        for (int j = 0; j < path.Count; j++)
                        {
                            Gizmos.DrawLine(path[i].position, path[j].position);
                        }
                    }
                    break;
            }
        }
    }
}