using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolingState : StateMachineBehaviour
{
    float timer;
    public float patrolingTime = 10f;

    Transform player;
    NavMeshAgent navAgent;

    public float detectionArea = 18f;
    public float patrolSpeed = 8f;

    List<Transform> waypointsList = new List<Transform>();

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // --- Initialization --- //

       player = GameObject.FindGameObjectWithTag("Player").transform;
       navAgent = animator.GetComponent<NavMeshAgent>();

       navAgent.speed = patrolSpeed;
       timer = 0;

       // --- Get all waypoints and Move to First Waypoint --- //
    
        GameObject waypointsCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach (Transform transform in waypointsCluster.transform)
        {
            waypointsList.Add(transform);
        }

        Vector3 nextPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        navAgent.SetDestination(nextPosition);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // --- If agent arrived at waypoint, move to next waypoint --- //

       if (navAgent.remainingDistance <= navAgent.stoppingDistance)
       {
            navAgent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
       }

       // --- Transition to Idle State --- //

       timer += Time.deltaTime;
       if (timer > patrolingTime)
       {
            animator.SetBool("isPatroling", false);
       }

        // --- Transition to Chase State --- //

       float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
       if (distanceFromPlayer < detectionArea)
       {
            animator.SetBool("isChasing", true);
       }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // --- Stop the agent --- //

       navAgent.SetDestination(navAgent.transform.position);
    }
}
