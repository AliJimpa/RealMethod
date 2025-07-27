using System.Collections;
using UnityEngine;

namespace RealMethod
{
    public static class RM_Coroutine
    {
        class CoroutineHandeler
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
        public static IEnumerator WaitForCoroutine(MonoBehaviour Owner, IEnumerator coroutine)
        {
            CoroutineHandeler handle = new CoroutineHandeler();
            yield return Owner.StartCoroutine(handle.Run(coroutine));
        }
    }

}