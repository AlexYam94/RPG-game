using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    [SerializeField] private bool hasAttacked = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    private void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        animator.ResetTrigger("attack1");
        animator.ResetTrigger("attack2");
        animator.ResetTrigger("attack3");
        hasAttacked=false;
        //reset number of attack
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        // Fighter fighter = animator.GetComponent<Fighter>();
        int numberOfAttack = animator.GetComponent<Fighter>().GetNumberOfAttack();
        if(numberOfAttack==1&&!hasAttacked){
            hasAttacked = true;
            animator.SetTrigger("attack"+numberOfAttack);
        }else
            hasAttacked = false;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) { 
        // int numberOfAttack = animator.GetComponent<Fighter>().GetNumberOfAttack();
        // animator.ResetTrigger("attack"+numberOfAttack);
        hasAttacked=false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
