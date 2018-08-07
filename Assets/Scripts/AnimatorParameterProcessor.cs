using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterProcessor : StateMachineBehaviour {

    [SerializeField] bool hurt;
    [SerializeField] bool knockback;
    [SerializeField] bool getUp;
    [SerializeField] bool magic;
    [SerializeField] bool attack;
    [SerializeField] bool attack3;
    [SerializeField] bool attack2;
    [SerializeField] bool charge;
    [SerializeField] bool mount;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (hurt)
            SetHurtBool(animator, false);

        if (knockback)
            SetKnockBackBool(animator, false);

        if (magic)
            animator.SetBool("magic", false);

        if (attack)
        {
            if (mount)
            {
                animator.gameObject.GetComponent<MountingController>().mountedCharacter.GetComponent<MOMovementController>().attackingAnim = true;
            }
            else
            {
                //animator.SetBool("attack", false);
                animator.gameObject.GetComponent<MOMovementController>().attackingAnim = true;

                if (attack3)
                    animator.gameObject.GetComponent<MOMovementController>().attackTrigger[0].GetComponent<Attack>().attack3 = true;

                if (attack2)
                    animator.gameObject.GetComponent<MOMovementController>().attackTrigger[0].GetComponent<Attack>().attack2 = true;
            }
        }

        if (charge)
            animator.SetBool("charge", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (mount)
        {
            animator.gameObject.GetComponent<MountingController>().mountedCharacter.GetComponent<MOMovementController>().attackingAnim = false;
        }
        else
        {
            animator.gameObject.GetComponent<MOMovementController>().attackingAnim = false;

            if (attack3)
                animator.gameObject.GetComponent<MOMovementController>().attackTrigger[0].GetComponent<Attack>().attack3 = false;

            if (attack2)
                animator.gameObject.GetComponent<MOMovementController>().attackTrigger[0].GetComponent<Attack>().attack2 = false;
        }

        if (getUp)
            animator.gameObject.GetComponent<MOMovementController>().knockedDownAnim = false;

        if (hurt)
            animator.gameObject.GetComponent<MOMovementController>().hurtAnim = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //      Add charge attack here
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    private void SetHurtBool (Animator animator, bool check)
    {
        animator.SetBool("hurt", check);
        animator.gameObject.GetComponent<MOMovementController>().hurtAnim = true;
    }

    private void SetKnockBackBool(Animator animator, bool check)
    {
        animator.SetBool("knockedDown", check);
        animator.gameObject.GetComponent<MOMovementController>().knockedDownAnim = true;
    }
}
