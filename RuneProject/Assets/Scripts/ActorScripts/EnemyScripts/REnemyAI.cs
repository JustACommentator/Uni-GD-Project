using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class REnemyMoveTo : MonoBehaviour
{

    [SerializeField] private Transform goal = null;
    [SerializeField] private double attackDistance = 3;

    [Header("References")]
    [SerializeField] private Rigidbody enemyRigidbody = null;
    [SerializeField] private NavMeshAgent agent = null;

    void Update()
    {
        agent.destination = goal.position;


        if (Vector3.Distance(enemyRigidbody.position, goal.position) < attackDistance)
        {
            //DoDamage();
        }
    }
}
