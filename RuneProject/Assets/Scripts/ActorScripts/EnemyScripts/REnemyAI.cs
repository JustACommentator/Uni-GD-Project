using RuneProject.ActorSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace RuneProject.EnemySystem
{
    /// <summary>
    /// Controlls the states of the enemy and makes it attack the target.
    /// </summary>
    public class REnemyAI : MonoBehaviour
    {
        [Header("States")]
        [SerializeField] private EAlertState currentState = EAlertState.IDLE;
        [SerializeField] private float susDistance = 5;
        [SerializeField] private float disengageDistance = 10;
        [SerializeField] private float disengageTime = 10;
        [SerializeField] private float aggroTime = 3;

        [Header("Combat")]
        [SerializeField] private EAttackPatternType currentPattern = EAttackPatternType.RUN_TOWARDS;
        [SerializeField] private float attackDistance = 2;
        [SerializeField] private float attackCooldownTime = 2;
        [SerializeField] private float tolleranceDistance = 1;
        [SerializeField] private float timeToleranceAttack = 1;
        [SerializeField] private float rotationSpeed = 720;
        [SerializeField] private Transform thrower = null;
        [SerializeField] private Rigidbody shoots = null;
        [SerializeField] private float throwingPower = 5;

        [Header("Patrol")]
        [SerializeField] private EPatrolMode currentPatrolState = EPatrolMode.DEFAULT;
        [SerializeField] private List<Transform> path = new List<Transform>();
        [SerializeField] private bool startAtFirstPathPosition = true;
        [SerializeField] private float reachedDistance = 1;

        [Header("References")]
        [SerializeField] private NavMeshAgent agent = null;
        [SerializeField] private Animator anim = null;
        [SerializeField] private List<GameObject> hitboxes = new List<GameObject>();
        [SerializeField] private RAttentionIndicator attentionIndicator = null;

        private float susCount = 0;
        private int currentPathPoint = 0;
        private bool pathForward = true;
        private float lastAttack = 0;
        private float randomTimeToAttack = 0;
        private Transform target = null;
        private RPlayerHealth player = null;

        private const float HITBOX_UPTIME = 0.5f;

        private EAlertState CurrentState {
            get => currentState;
            set {
                currentState = value;
                attentionIndicator.switchState(value);
            }
        }

        public List<Transform> Path { get => path; set => path = value; }

        public enum EAlertState
        {
            IDLE,
            SUSPICIOUS,
            AGGRESSIVE
        }

        enum EPatrolMode
        {
            DEFAULT,
            TRACE_BACK,
            PING_PONG,
            RANDOM
        }

        enum EAttackPatternType
        {
            RUN_TOWARDS,
            CIRCLE,
            RUN_AWAY
        }

        private void Start()
        {
            currentState = EAlertState.IDLE;

            if (path.Count > 1 && startAtFirstPathPosition)
                transform.position = path[0].position;

            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<RPlayerHealth>();
                target = player?.transform;
            }

            for (int i = 0; i < hitboxes.Count; i++)
                hitboxes[i].SetActive(false);
        }

        private void Update()
        {
            if (!player)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<RPlayerHealth>();
                target = player?.transform;
            }

            if (!player.IsAlive)
            {
                CurrentState = EAlertState.IDLE;
                lastAttack = 0;
                return;
            }

            float distToTarget = Vector3.Distance(transform.position, target.position);

            switch (CurrentState)
            {
                case EAlertState.IDLE:

                    if (distToTarget < susDistance)
                    {
                        CurrentState = EAlertState.SUSPICIOUS;
                        return;
                    }

                    if (path.Count > 1 && Vector3.Distance(transform.position, path[currentPathPoint].position) <= reachedDistance)
                    {
                        switch (currentPatrolState)
                        {
                            case EPatrolMode.DEFAULT:
                                currentPathPoint = (currentPathPoint + 1) % path.Count;
                                break;

                            case EPatrolMode.TRACE_BACK:
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

                            case EPatrolMode.PING_PONG:
                                if (currentPathPoint == 0)                                        
                                    currentPathPoint = Random.Range(1, path.Count);                                        
                                else                                        
                                    currentPathPoint = 0;                                                                           
                                break;

                            case EPatrolMode.RANDOM:
                                int newPathPoint = Random.Range(0, path.Count);
                                while (currentPathPoint == newPathPoint)
                                {
                                    newPathPoint = Random.Range(0, path.Count);
                                }
                                currentPathPoint = newPathPoint;
                                break;
                        }

                        if (agent.isOnNavMesh)
                            agent.SetDestination(path[currentPathPoint].position);
                    }                    

                    break;

                case EAlertState.SUSPICIOUS:

                    if (distToTarget < susDistance)
                    {
                        susCount = Mathf.Clamp(susCount + (1 / aggroTime) * Time.deltaTime, 0, 1);
                    }
                    else
                    {
                        susCount = Mathf.Clamp(susCount - (1 / aggroTime) * Time.deltaTime, 0, 1);
                    }

                    if (distToTarget < attackDistance)
                    {
                        susCount = 1;
                    }

                    if (susCount == 1)
                    {
                        CurrentState = EAlertState.AGGRESSIVE;
                    }

                    if (susCount == 0)
                    {
                        CurrentState = EAlertState.IDLE;
                        currentPathPoint = 0;
                        if (agent.isOnNavMesh && path.Count > 0)
                            agent.SetDestination(path[0].position);
                    }

                    break;

                case EAlertState.AGGRESSIVE:

                    lastAttack += Time.deltaTime;

                    if (distToTarget > disengageDistance || lastAttack > disengageTime)
                    {
                        CurrentState = EAlertState.SUSPICIOUS;
                        lastAttack = 0;
                        return;
                    }

                    Vector3 targetDirection = Vector3.Normalize(transform.position - target.position);

                    switch (currentPattern)
                    {

                        case EAttackPatternType.CIRCLE:

                            targetDirection = Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up) * targetDirection;

                            goto case EAttackPatternType.RUN_TOWARDS;

                        case EAttackPatternType.RUN_TOWARDS:

                            if (distToTarget < attackDistance && lastAttack > attackCooldownTime + randomTimeToAttack)
                            {
                                //DoDamage()
                                if (shoots)
                                {
                                    Rigidbody projectile = Instantiate(shoots, thrower.position, thrower.rotation);
                                    projectile.velocity = Vector3.Normalize(target.position + Vector3.up * Mathf.Sqrt(distToTarget) - transform.position) * throwingPower;
                                }
                                else if (agent.isOnNavMesh)
                                {
                                    anim.SetTrigger("attack");
                                    agent.destination = target.position + targetDirection * 0.3f;
                                    StartCoroutine(IToggleHitboxes());
                                }

                                lastAttack = 0;
                                randomTimeToAttack = Random.value * timeToleranceAttack;
                            }
                            else if ((distToTarget < attackDistance - tolleranceDistance || distToTarget > attackDistance - 0.1f) && lastAttack > 0.1 && agent.isOnNavMesh)
                            {
                                agent.destination = target.position + targetDirection * Random.Range(attackDistance / 2, attackDistance - 0.1f);
                            }

                            break;

                        case EAttackPatternType.RUN_AWAY:

                            if (distToTarget < attackDistance && agent.isOnNavMesh)
                                agent.destination = target.position + targetDirection * disengageDistance;

                            break;

                    }

                    break;
            }

            Vector3 dir;

            if (CurrentState != EAlertState.AGGRESSIVE)
                dir = agent.velocity;
            else
                dir = target.position - transform.position;

            dir.y = 0;
            if (dir != Vector3.zero)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Lerp(transform.rotation, rot, agent.angularSpeed * 0.1f * Time.deltaTime);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, susDistance);

            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, disengageDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * reachedDistance);

            if (path.Count > 1)
                Gizmos.DrawLine(transform.position, path[currentPathPoint].position);

            switch(currentPatrolState)
            {
                case EPatrolMode.DEFAULT:
                    for (int i = 0; i < path.Count; i++)
                    {
                        Gizmos.DrawLine(path[i].position, path[(i + 1) % path.Count].position);
                    }
                    break;

                case EPatrolMode.TRACE_BACK:
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        Gizmos.DrawLine(path[i].position, path[(i + 1) % path.Count].position);
                    }
                    break;

                case EPatrolMode.PING_PONG:
                    for (int i = 0; i < path.Count; i++)
                    {
                        Gizmos.DrawLine(path[0].position, path[i].position);
                    }
                    break;

                case EPatrolMode.RANDOM:
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

        private IEnumerator IToggleHitboxes()
        {
            for (int i=0; i<hitboxes.Count; i++)
                hitboxes[i].SetActive(true);
            yield return new WaitForSeconds(HITBOX_UPTIME);
            for (int i = 0; i < hitboxes.Count; i++)
                hitboxes[i].SetActive(false);
        }
    }
}