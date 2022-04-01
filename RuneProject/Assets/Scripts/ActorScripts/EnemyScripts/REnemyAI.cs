using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class REnemyAI : MonoBehaviour
{

    [SerializeField] private Transform goal = null;
    [SerializeField] private double attackDistance = 3;

    [Header("References")]
    [SerializeField] private Rigidbody enemyRigidbody = null;
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private State currentState = State.idle;
    [SerializeField] private float susDistance = 5;

    private float susCount = 0;
    private float agressiveness = 0.01f;

    enum State
    {
        idle,
        suspicious,
        aggro
    }


    void Update()
    {
        switch (currentState)
        {
            case State.idle:

                if (Vector3.Distance(enemyRigidbody.position, goal.position) < susDistance)
                {
                    currentState = State.suspicious;
                }
                break;

            case State.suspicious:

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
                    currentState = State.aggro;
                }
                break;

            case State.aggro:

                agent.destination = goal.position;

                if (Vector3.Distance(enemyRigidbody.position, goal.position) < attackDistance)
                {
                    //DoDamage();
                }
                break;

            default:
                break;
        }
    }
}
