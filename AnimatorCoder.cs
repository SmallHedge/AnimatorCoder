//Author: Small Hedge Games
//Date: 02/07/2024

using System.Collections;
using UnityEngine;
using System;

namespace SHG.AnimatorCoder
{
    public abstract class AnimatorCoder : MonoBehaviour
    {
        /// <summary> The baseline animation logic on a specific layer </summary>
        public abstract void DefaultAnimation(int layer);
        private Animator animator = null;
        private Animations[] currentAnimation;
        private bool[] layerLocked;
        private ParameterDisplay[] parameters;
        private Coroutine[] currentCoroutine;

        /// <summary> Sets up the Animator Brain </summary>
        public void Initialize(Animator animator = null)
        {
            AnimatorValues.Initialize();

            if(animator == null)
                this.animator = GetComponent<Animator>();
            else
                this.animator = animator;
                
            currentCoroutine = new Coroutine[this.animator.layerCount];
            layerLocked = new bool[this.animator.layerCount];
            currentAnimation = new Animations[this.animator.layerCount];

            for (int i = 0; i < this.animator.layerCount; ++i)
            {
                layerLocked[i] = false;

                int hash = this.animator.GetCurrentAnimatorStateInfo(i).shortNameHash;
                for (int k = 0; k < AnimatorValues.Animations.Length; ++k)
                {
                    if (hash == AnimatorValues.Animations[k])
                    {
                        currentAnimation[i] = (Animations)Enum.GetValues(typeof(Animations)).GetValue(k);
                        k = AnimatorValues.Animations.Length;
                    }
                }
            }

            string[] names = Enum.GetNames(typeof(Parameters));
            parameters = new ParameterDisplay[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                parameters[i].name = names[i];
                parameters[i].value = false;
            }
        }

        /// <summary> Returns the current animation that is playing </summary>
        public Animations GetCurrentAnimation(int layer)
        {
            try
            {
                return currentAnimation[layer];
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
                return Animations.RESET;
            }
        }

        /// <summary> Sets the whole layer to be locked or unlocked </summary>
        public void SetLocked(bool lockLayer, int layer)
        {
            try
            {
                layerLocked[layer] = lockLayer;
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
            }
        }

        public bool IsLocked(int layer)
        {
            try
            {
                return layerLocked[layer];
            }
            catch
            {
                LogError("Can't retrieve Current Animation. Fix: Initialize() in Start() and don't exceed number of animator layers");
                return false;
            }
        }

        /// <summary> Sets an animator parameter </summary>
        public void SetBool(Parameters id, bool value)
        {
            try
            {
                parameters[(int)id].value = value;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
            }
        }

        /// <summary> Returns an animator parameter </summary>
        public bool GetBool(Parameters id)
        {
            try
            {
                return parameters[(int)id].value;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
                return false;
            }
        }

        /// <summary> Takes in the animation details and the animation layer, then attempts to play the animation </summary>
        public bool Play(AnimationData data, int layer = 0)
        {
            try
            {
                if (data.animation == Animations.RESET)
                {
                    DefaultAnimation(layer);
                    return false;
                }

                if (layerLocked[layer] || currentAnimation[layer] == data.animation) return false;

                if (currentCoroutine[layer] != null) StopCoroutine(currentCoroutine[layer]);
                layerLocked[layer] = data.lockLayer;
                currentAnimation[layer] = data.animation;

                animator.CrossFade(AnimatorValues.GetHash(currentAnimation[layer]), data.crossfade, layer);

                if (data.nextAnimation != null)
                {
                    currentCoroutine[layer] = StartCoroutine(Wait());
                    IEnumerator Wait()
                    {
                        animator.Update(0);
                        float delay = animator.GetNextAnimatorStateInfo(layer).length;
                        if (data.crossfade == 0) delay = animator.GetCurrentAnimatorStateInfo(layer).length;
                        yield return new WaitForSeconds(delay - data.nextAnimation.crossfade);
                        SetLocked(false, layer);
                        Play(data.nextAnimation, layer);
                    }
                }

                return true;
            }
            catch
            {
                LogError("Please Initialize() in Start()");
                return false;
            }
        }

        private void LogError(string message)
        {
            Debug.LogError("AnimatorCoder Error: " + message);
        }
    }

    /// <summary> Holds all data about an animation </summary>
    [Serializable]
    public class AnimationData
    {
        public Animations animation;
        /// <summary> Should the layer lock for this animation? </summary>
        public bool lockLayer;
        /// <summary> Should an animation play immediately after? </summary>
        public AnimationData nextAnimation;
        /// <summary> Should there be a transition time into this animation? </summary>
        public float crossfade = 0;

        /// <summary> Sets the animation data </summary>
        public AnimationData(Animations animation = Animations.RESET, bool lockLayer = false, AnimationData nextAnimation = null, float crossfade = 0)
        {
            this.animation = animation;
            this.lockLayer = lockLayer;
            this.nextAnimation = nextAnimation;
            this.crossfade = crossfade;
        }
    }

    /// <summary> Class the manages the hashes of animations and parameters </summary>
    public class AnimatorValues
    {
        /// <summary> Returns the animation hash array </summary>
        public static int[] Animations { get { return animations; } }

        private static int[] animations;
        private static bool initialized = false;

        /// <summary> Initializes the animator state names </summary>
        public static void Initialize()
        {
            if (initialized) return;
            initialized = true;

            string[] names = Enum.GetNames(typeof(Animations));

            animations = new int[names.Length];
            for (int i = 0; i < names.Length; i++)
                animations[i] = Animator.StringToHash(names[i]);
        }

        /// <summary> Gets the animator hash value of an animation </summary>
        public static int GetHash(Animations animation)
        {
            return animations[(int)animation];
        }
    }

    /// <summary> Allows the animation parameters to be shown in debug inspector </summary>
    [Serializable]
    public struct ParameterDisplay
    {
        [HideInInspector] public string name;
        public bool value;
    }
}
