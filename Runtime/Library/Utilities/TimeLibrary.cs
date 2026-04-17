using UnityEngine;
using System.Collections;


namespace RealMethod
{
    public static class RM_Time
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


        /// <summary>
        /// Waits for a specified duration and then invokes a callback.
        /// </summary>
        /// <param name="duration">Time in seconds to wait before executing the callback.</param>
        /// <param name="callback">The action to invoke after the delay.</param>
        /// <returns>
        /// An IEnumerator that can be used in a Unity coroutine.
        /// </returns>
        /// <remarks>
        /// Must be started using <c>StartCoroutine()</c>.
        /// </remarks>
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
        public static IEnumerator DelayOneFrame(System.Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback?.Invoke();
        }
        public static IEnumerator WaitForCoroutine(MonoBehaviour Owner, IEnumerator coroutine)
        {
            CoroutineHandeler handle = new CoroutineHandeler();
            yield return Owner.StartCoroutine(handle.Run(coroutine));
        }
    }
}