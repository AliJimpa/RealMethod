using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Coroutine
    {
        private class CoroutineHandeler
        {
            public bool IsDone { get; private set; }

            public IEnumerator Run(IEnumerator coroutine)
            {
                yield return coroutine;
                IsDone = true;
            }
        }


        public static IEnumerator Delay(float duration, System.Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
        public static IEnumerator WaitUntil(System.Func<bool> condition, System.Action callback)
        {
            yield return new WaitUntil(condition);
            callback?.Invoke();
        }
        public static IEnumerator WaitWhile(System.Func<bool> condition, System.Action callback)
        {
            yield return new WaitWhile(condition);
            callback?.Invoke();
        }
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
        public static IEnumerator Repeat(System.Action action, float interval, int count = -1)
        {
            int i = 0;
            while (count < 0 || i < count)
            {
                action?.Invoke();
                i++;
                yield return new WaitForSeconds(interval);
            }
        }
        public static IEnumerator WaitForCoroutine(MonoBehaviour Owner, IEnumerator coroutine)
        {
            CoroutineHandeler handle = new CoroutineHandeler();
            yield return Owner.StartCoroutine(handle.Run(coroutine));
        }
        private static IEnumerator DelayOneFrame(System.Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }
    }

}