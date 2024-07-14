using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChasingState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent navAgent;

    public float chaseSpeed = 6f;

    public float stopChasingDistance = 21f;
    public float attackingDistance = 2.5f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // --- Initialization --- //

       player = GameObject.FindGameObjectWithTag("Player").transform;
       navAgent = animator.GetComponent<NavMeshAgent>();

       navAgent.speed = chaseSpeed;

       //SoundManager.Instance.enemyChannel.PlayOneShot(SoundManager.Instance.enemyChase);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navAgent.SetDestination(player.position);
       animator.transform.LookAt(player);

       float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

       // --- Checking if the agent should stop Chasing --- //

        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
        }

       // --- Checking if the agent should Attack --- //
       if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       navAgent.SetDestination(navAgent.transform.position);
    }
}
