﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterProcessor : StateMachineBehaviour {

    [SerializeField] bool hurt;
    [SerializeField] bool knockback;
    [SerializeField] bool magic;
    [SerializeField] bool attack;

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
            animator.SetBool("attack", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (hurt)
        //    SetHurtBool(animator, false);

        //if (knockback)
        //    SetKnockBackBool(animator, false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    private void SetHurtBool (Animator animator, bool check)
    {
        animator.SetBool("hurt", check);
    }

    private void SetKnockBackBool(Animator animator, bool check)
    {
        animator.SetBool("knockedDown", check);
    }
}
