using UnityEngine;

namespace RealMethod
{
    public static class Animator_Extension
    {
        public static Coroutine PlayAndNotify(this Animator animator, string stateName, System.Action onFinished, int layer = 0)
        {
            animator.Play(stateName, layer);
            // Start a coroutine to watch for finish
            var runner = animator.GetComponent<MonoBehaviour>();
            if (runner == null)
            {
                Debug.LogWarning("For Use PlayAndNotify you need to a MonoBehaviour for running Cortine for Finish Callback");
                return null;
            }
            return runner.StartCoroutine(RM_Coroutine.WaitForState(animator, layer, stateName, onFinished));
        }

        /// <summary>
        /// Checks if the Animator is currently playing a specific state.
        /// </summary>
        public static bool IsPlaying(this Animator animator, string stateName, int layer = 0)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);
            return info.IsName(stateName) && info.normalizedTime < 1f;
        }

        /// <summary>
        /// Checks if the Animator is in a specific state (finished or still playing).
        /// </summary>
        public static bool IsInState(this Animator animator, string stateName, int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName);
        }

        /// <summary>
        /// Returns the normalized progress of the current animation state (0 → 1).
        /// </summary>
        public static float GetProgress(this Animator animator, int layer = 0)
        {
            return animator.GetCurrentAnimatorStateInfo(layer).normalizedTime;
        }

        /// <summary>
        /// Safely sets a trigger (resets it first to avoid stuck transitions).
        /// </summary>
        public static void SetTriggerSafe(this Animator animator, string triggerName)
        {
            animator.ResetTrigger(triggerName);
            animator.SetTrigger(triggerName);
        }

        /// <summary>
        /// Immediately stops all animations and plays the given state.
        /// </summary>
        public static void ForcePlay(this Animator animator, string stateName, int layer = 0, float normalizedTime = 0f)
        {
            animator.Play(stateName, layer, normalizedTime);
            animator.Update(0f); // force immediate update so the state is applied instantly
        }

        /// <summary>
        /// Checks if the current state is about to end (useful for chaining combos).
        /// </summary>
        public static bool IsNearEnd(this Animator animator, float threshold = 0.9f, int layer = 0)
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);
            return info.normalizedTime >= threshold && !info.loop;
        }

        /// <summary>
        /// Crossfades safely only if the target state isn’t already playing.
        /// </summary>
        public static void CrossFadeIfNotPlaying(this Animator animator, string stateName, float transitionDuration = 0.1f, int layer = 0)
        {
            if (!animator.IsInState(stateName, layer))
                animator.CrossFadeInFixedTime(stateName, transitionDuration, layer);
        }
    }
}