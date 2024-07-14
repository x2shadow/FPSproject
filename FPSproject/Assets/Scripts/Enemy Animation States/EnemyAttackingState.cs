using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttackingState : StateMachineBehaviour
{
    Transform player;
    NavMeshAgent navAgent;
    
    public float stopAttackingDistance = 2.5f;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       // --- Initialization --- //

       player = GameObject.FindGameObjectWithTag("Player").transform;
       navAgent = animator.GetComponent<NavMeshAgent>();

        SoundManager.Instance.enemyChannel.PlayOneShot(SoundManager.Instance.enemyAttacking);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SoundManager.Instance.enemyChannel.isPlaying == false)
        {
            //SoundManager.Instance.enemyChannel.PlayOneShot(SoundManager.Instance.enemyAttacking);
        }

        LookAtPlayer();
  
        // --- Check if the agent should stop attacking --- //

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void LookAtPlayer()
    {
        Vector3 direction = player.position - navAgent.transform.position;
        navAgent.transform.rotation = Quaternion.LookRotation(direction);
        
        // Extract y and set x and z to 0
        var yRotation = navAgent.transform.eulerAngles.y;
        navAgent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
