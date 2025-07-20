using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Coroutine
    {
        public static IEnumerator Delay(float duration, System.Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback?.Invoke();
        }
        public static IEnumerator WaitForCoroutine(MonoBehaviour Owner, IEnumerator coroutine)
        {
            CoroutineSequencer handle = new CoroutineSequencer();
            yield return Owner.StartCoroutine(handle.Run(coroutine));
        }
    }

}