//Author: Small Hedge Games
//Date: 05/04/2024

using UnityEngine;

namespace SHG.AnimatorCoder
{
    public class OnParameter : StateMachineBehaviour
    {
        [SerializeField, Tooltip("Parameter to test")] private Parameters parameter;
        [SerializeField, Tooltip("Specify whether it should be on or off")] private bool target;
        [SerializeField, Tooltip("Chain of animations to play when condition is met")] private AnimationData[] nextAnimations;

        private AnimatorCoder animatorBrain;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animatorBrain = animator.GetComponent<AnimatorCoder>();
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animatorBrain.GetBool(parameter) != target) return;
            animatorBrain.SetLocked(false, layerIndex);

            for (int i = 0; i < nextAnimations.Length - 1; ++i)
                nextAnimations[i].nextAnimation = nextAnimations[i + 1];

            animatorBrain.Play(nextAnimations[0], layerIndex);
        }
    }
}

