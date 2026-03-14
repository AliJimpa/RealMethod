using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class Tween
    {
        /// <summary>
        /// Moves the target transform to a specified position over a given duration.
        /// </summary>
        /// <param name="target">The transform to move.</param>
        /// <param name="end">The target position.</param>
        /// <param name="duration">The time in seconds to complete the movement.</param>
        /// <returns>An IEnumerator for use with StartCoroutine.</returns>
        /// <remarks>
        /// Should be started with <c>StartCoroutine()</c>.
        /// </remarks>
        public static IEnumerator MoveTo(Transform target, Vector3 end, float duration)
        {
            Vector3 start = target.position;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                target.position = Vector3.Lerp(start, end, t / duration);
                yield return null;
            }
            target.position = end;
        }
        public static IEnumerator FadeCanvas(CanvasGroup canvas, float targetAlpha, float duration)
        {
            float start = canvas.alpha;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                canvas.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
                yield return null;
            }
            canvas.alpha = targetAlpha;
        }
        public static IEnumerator ScaleTo(Transform target, Vector3 targetScale, float duration)
        {
            Vector3 start = target.localScale;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                target.localScale = Vector3.Lerp(start, targetScale, t / duration);
                yield return null;
            }
            target.localScale = targetScale;
        }
        public static IEnumerator RotateTo(Transform target, Quaternion targetRot, float duration)
        {
            Quaternion start = target.rotation;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                target.rotation = Quaternion.Slerp(start, targetRot, t / duration);
                yield return null;
            }
            target.rotation = targetRot;
        }

    }
}