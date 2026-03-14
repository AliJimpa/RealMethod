using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Animation
    {
        /// <summary>
        /// Waits until the specified state is entered on an Animator layer,
        /// then invokes a callback once the state becomes active.
        /// </summary>
        /// <param name="animator">The Animator to monitor.</param>
        /// <param name="layer">The Animator layer index to check.</param>
        /// <param name="stateName">The name of the state to wait for.</param>
        /// <param name="onFinished">Callback invoked when the target state is reached.</param>
        /// <returns>
        /// An <see cref="IEnumerator"/> that can be used with <c>StartCoroutine()</c>.
        /// </returns>
        /// <remarks>
        /// Useful for sequencing animations without hard‑coded delays.
        /// Ensure that the Animator contains the target state on the specified layer.
        /// </remarks>
        public static IEnumerator WaitForState(Animator animator, int layer, string stateName, System.Action onFinished)
        {
            // Wait until we actually enter the state
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(layer);
            while (!info.IsName(stateName))
            {
                yield return null;
                info = animator.GetCurrentAnimatorStateInfo(layer);
            }

            // Now wait until state finishes (non-looping assumption)
            while (info.normalizedTime < 1f)
            {
                yield return null;
                info = animator.GetCurrentAnimatorStateInfo(layer);
            }

            onFinished?.Invoke();
        }
    }
}