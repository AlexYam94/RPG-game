using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using UnityEngine;

public class AttackBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("attack"+animator.GetComponent<Fighter>().GetNumberOfAttack());
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        int numberOfAttack = animator.GetComponent<Fighter>().GetNumberOfAttack();
        if(numberOfAttack>1){
            animator.SetTrigger("attack"+numberOfAttack);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
    private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex) {
        int numberOfAttack = animator.GetComponent<Fighter>().GetNumberOfAttack();
        animator.ResetTrigger("attack"+(numberOfAttack-1));
        animator.ResetTrigger("attack"+numberOfAttack);
        
        //reset number of attack if numberOfAttack == 3?
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
